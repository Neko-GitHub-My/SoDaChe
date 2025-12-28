using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.WebSockets;
using Framework.Core.Serializer;
using System.Net.Sockets;
using System.Net;
using System;
using System.Threading;
using Unity.VisualScripting;

public class WebSocketWrapper : ISocketWrapper
{
    private ClientWebSocket ws = null;
    private string wsUrl = null;

    private Thread recvThread = null; 

    private State state = State.Disconnect;

    private Queue<Message> netEvents = new Queue<Message>();

    private const int RECV_LEN = 8192;
    private byte[] recvBuf = new byte[RECV_LEN];

    private const int HEAD_SIZE = 14; // [2, 2, 8, 2]

    public void Init(string wsUrl)
    {
        this.ws = null;
        this.wsUrl = wsUrl;

        this.state = State.Invalid;

    }

    void OnConnectError(string err)
    {
        this.state = State.Disconnect;
        EventMgr.Instance.Emit(SocketMgr.NetDisconnectEvent, -1);
    }

    public async void ConnectToServer()
    {
        if (this.state != State.Disconnect && this.state != State.Invalid)
        {
            return;
        }

        this.state = State.Connecting;
        EventMgr.Instance.Emit(SocketMgr.NetConnectingEvent, null);

        this.ws = new ClientWebSocket();
        try
        {
            await this.ws.ConnectAsync(new Uri(this.wsUrl), CancellationToken.None);
        }
        catch (Exception e)
        {
            this.OnConnectError(e.ToString());
            return;
        }
        

        this.state = State.Connected;
        EventMgr.Instance.Emit(SocketMgr.NetConnectedEvent, null);

        this.recvThread = new Thread(new ThreadStart(this.ThreadRecvWorker));
        this.recvThread.Start();
    }

    void OnRecvCmd(byte[] data, int offset, int dataLen)
    {
        if (dataLen <= 0)
        {
            return;
        }


        short module = DataViewer.ReadShortLE(data, offset + 0); // LE的方式
        short cmd = DataViewer.ReadShortLE(data, offset + 2);
        
        Message msg = SerializerHelper.PbDecode((short)(module), cmd, data, offset + HEAD_SIZE, dataLen - HEAD_SIZE);


        {
            lock (this.netEvents)
            { // recv thread
                this.netEvents.Enqueue(msg);
            }
        }
    }

    async void ThreadRecvWorker()
    {
        
        while (true)
        {
            if (this.ws.State != WebSocketState.Open) {
                break;
            }
            try {
                WebSocketReceiveResult result = await this.ws.ReceiveAsync(new ArraySegment<byte>(this.recvBuf), CancellationToken.None);
                if (result != null)
                {
                    if (result.Count > 0)
                    {
                        this.OnRecvCmd(this.recvBuf, 0, result.Count);
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            } catch (Exception e) {
                this.state = State.Disconnect;
                Debug.Log(e.ToString());
                this.ws.Abort();
                this.ws.Dispose();
                Debugger.Log("exit WebSocket recv thread");
                return;
            }
            
        }

        this.state = State.Disconnect;

        this.ws.Abort();
        this.ws.Dispose();
        Debugger.Log("exit WebSocket recv thread");
    }

    public void SendMsg(Message msg, short machineId, long utag = 0) {
        if (this.state != State.Connected)
        {
            return;
        }

        var msgBody = SerializerHelper.PBEncoder(msg);
        if (msgBody == null)
        {
            return;
        }

        byte[] body = new byte[HEAD_SIZE + msgBody.Length];

        short module = msg.GetModule();
        module = (short)((int)(module) | ((int)machineId << 8));
        DataViewer.WriteShortLE(body, 0, module);
        DataViewer.WriteShortLE(body, 2, msg.GetCmd());
        DataViewer.WriteULongLE(body, 4, (ulong)utag);
        DataViewer.WriteBytes(body, HEAD_SIZE, msgBody);

        this.ws.SendAsync(new ArraySegment<byte>(body), WebSocketMessageType.Binary, true, CancellationToken.None);
    }

    public void Close()
    {
        if (this.state != State.Connected) {
            return;
        }

        this.state = State.Disconnect;
        
        EventMgr.Instance.Emit(SocketMgr.NetDisconnectEvent, 1);

        // abort recv thread
        if (this.recvThread != null)
        {
            this.recvThread.Interrupt();
            this.recvThread.Abort();

            this.recvThread.Join();
            this.recvThread = null;
        }
        // end

        if (this.ws != null && this.ws.State == WebSocketState.Open)
        {
            this.ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None).Wait();
            this.ws.Abort();
            this.ws.Dispose();
            this.ws = null;
        }

        // 等待线程结束
        // end
    }

    public void Update()
    {
        if (this.state == State.Disconnect)
        {
            this.ConnectToServer();
            return;
        }

        
        lock (this.netEvents)
        {
            while (this.netEvents.Count > 0)
            {
                Message body = this.netEvents.Dequeue();
                // 出发网络事件
                EventMgr.Instance.Emit(SocketMgr.NetRecvCmdEvent, body);
                // end
            }
        }
    }
}



