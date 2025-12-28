using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Core.Serializer;



class TcpPacker
{
    private const int HEADER_SIZE = 2;
    public static byte[] pack(byte[] cmd_data)
    {
        int len = cmd_data.Length;
        if (len > 65535 - HEADER_SIZE) {
            return null;
        }

        int cmd_len = len + HEADER_SIZE;
        byte[] cmd = new byte[cmd_len];
        DataViewer.WriteUShortLE(cmd, 0, (ushort)cmd_len);
        DataViewer.WriteBytes(cmd, HEADER_SIZE, cmd_data);

        // 最后你可以写校验数据字节
        // end
        return cmd;
    }

    public static bool ReadHeader(byte[] data, int data_len, out int pkg_size, out int head_size)
    {
        pkg_size = 0;
        head_size = 0;

        if (data_len < 2)
        {
            return false;
        }

        head_size = 2;
        pkg_size = (data[0] | (data[1] << 8));

        return true;
    }

    public static void WriteShortLE(byte[] data, int offset, short value)
    {
        data[offset + 0] = (byte)((value & 0x00ff));
        data[offset + 1] = (byte)((value & 0xff00) >> 8);
    }

    public static void WriteUintLE(byte[] data, int offset, uint value)
    {
        data[offset + 0] = (byte)((value & 0x000000ff));
        data[offset + 1] = (byte)((value & 0x0000ff00) >> 8);

        data[offset + 2] = (byte)((value & 0x00ff0000) >> 16);
        data[offset + 3] = (byte)((value & 0xff000000) >> 24);
    }

    public static void WriteBytes(byte[] dst, int offset, byte[] src)
    {
        Array.Copy(src, 0, dst, offset, src.Length);
    }

    public static short ReadShortLE(byte[] data, int offset)
    {
        short value = (short)((data[offset + 1] << 8) | (data[offset + 0]));
        return value;
    }

    public static uint ReadUintLE(byte[] data, int offset)
    {
        uint value = (uint)((data[offset + 3] << 24) | (data[offset + 2] << 16) | (data[offset + 1] << 8) | (data[offset + 0]));
        return value;
    }
}

public class TcpSocketWrapper : ISocketWrapper
{
    

    private string serverIp = "127.0.0.1";
    private int port = 6080;

    private Socket clientSocket = null;
    private bool isConnected = false;
    private Thread recvThread = null;

    private const int RECV_LEN = 8192;
    private byte[] recvBuf = new byte[RECV_LEN];
    private int recved = 0;
    private byte[] longPkg = null;
    private int longPkgSize = 0;
    // end

    private State state = State.Disconnect;

    private const int HEAD_SIZE = 14; // [2, 2, 8, 2]

    private Queue<Message> netEvents = new Queue<Message>();
    

    public void Init(string serverIp, int port) {
        this.serverIp = serverIp;
        this.port = port;
        this.state = State.Invalid;
        // this.state = State.Disconnect;
        // this.ConnectToServer(); // 可以考虑，自己在外面调用;
    }

    
	public void Update () {
        if (this.state == State.Disconnect) {
            this.ConnectToServer();
            return;
        }

        lock (this.netEvents) {
            while (this.netEvents.Count > 0) {
                Message body = this.netEvents.Dequeue();
                // 出发网络事件
                EventMgr.Instance.Emit(SocketMgr.NetRecvCmdEvent, body);
                // end
            }
        }
	}

    void OnConnectError(string err) {
        this.state = State.Disconnect;
        EventMgr.Instance.Emit(SocketMgr.NetDisconnectEvent, -1);
    }

    public void ConnectToServer() {
        if (this.state != State.Disconnect && this.state != State.Invalid) {
            return;
        }

        this.state = State.Connecting;
        EventMgr.Instance.Emit(SocketMgr.NetConnectingEvent, null);

        try {
            this.clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAddress = IPAddress.Parse(this.serverIp);
            IPEndPoint ipEndpoint = new IPEndPoint(ipAddress, this.port);

            this.clientSocket.BeginConnect(ipEndpoint, new AsyncCallback(this.OnConnected), this.clientSocket);
        }
        catch (System.Exception e) {
            Debug.Log(e.ToString());
            this.OnConnectError(e.ToString());
        }
    }

    void OnRecvCmd(byte[] data, int offset, int dataLen) {
        if (dataLen <= 0) {
            return;
        }


        /*byte[] msg = new byte[dataLen];
        Array.Copy(data, start, msg, 0, dataLen);
        */

        short module = DataViewer.ReadShortLE(data, offset + 0); // LE的方式
        short cmd = DataViewer.ReadShortLE(data, offset + 2);
        Message msg = SerializerHelper.PbDecode((short)(module), cmd, data, offset + HEAD_SIZE, dataLen - HEAD_SIZE);


        { 
            lock (this.netEvents) { // recv thread
                this.netEvents.Enqueue(msg);
            }
        }
    }

    void OnRecvTcpData() { 
        byte[] pkgData = (this.longPkg != null) ? this.longPkg : this.recvBuf;
        while (this.recved > 0) {
			int pkgSize = 0;
			int headSize = 0;

			if (!TcpPacker.ReadHeader(pkgData, this.recved, out pkgSize, out headSize)) {
				break;
			}

			if (this.recved < pkgSize) {
				break;
			}

            int rawDataStart = headSize;
            int rawDataLen = pkgSize - headSize;

            OnRecvCmd(pkgData, rawDataStart, rawDataLen);
			
			if (this.recved > pkgSize) {
                this.recvBuf = new byte[RECV_LEN];
				Array.Copy(pkgData, pkgSize, this.recvBuf, 0, this.recved - pkgSize);
                pkgData = this.recvBuf;
			}

			this.recved -= pkgSize;

			if (this.recved == 0 && this.longPkg != null) {
				this.longPkg = null;
                this.longPkgSize = 0;
			}
		}
    }

    void ThreadRecvWorker() {
        if (this.isConnected == false) {
            return;
        }

        while (true) {
            if (!this.clientSocket.Connected) {
                break;
            }

            try
            {
                int recvLen = 0; 
                if (this.recved < RECV_LEN) {
                    recvLen = this.clientSocket.Receive(this.recvBuf, this.recved, RECV_LEN - this.recved, SocketFlags.None);
                }
                else {
                    if (this.longPkg == null) {
                        int pkgSize;
                        int headSize;
                        TcpPacker.ReadHeader(this.recvBuf, this.recved, out pkgSize,  out headSize);
                        this.longPkgSize = pkgSize;
                        this.longPkg = new byte[pkgSize];
                        Array.Copy(this.recvBuf, 0, this.longPkg, 0, this.recved);
                    }
                    recvLen = this.clientSocket.Receive(this.longPkg, this.recved, this.longPkgSize - this.recved, SocketFlags.None);
                }

                if (recvLen > 0) {
                    this.recved += recvLen;
                    this.OnRecvTcpData();
                }
            }
            catch (System.Exception e) {
                Debug.Log(e.ToString());
                if (this.clientSocket != null && this.clientSocket.Connected) {
                    /*if(this.clientSocket.Connected) {
                        this.clientSocket.Disconnect(true);
                    }*/
                    this.clientSocket.Shutdown(SocketShutdown.Both);
                    this.clientSocket.Close();
                }

                Debugger.Log("exit recv thread ^^^^^^^^^^^");
                this.clientSocket = null; 
                this.isConnected = false;
                this.state = State.Disconnect;
                break;
            }
        }

        Debugger.Log("exit recv thread");
    }

    void OnConnected(IAsyncResult iar) {
        try {
            Socket client = (Socket)iar.AsyncState;
            client.EndConnect(iar);

            this.state = State.Connected;
            EventMgr.Instance.Emit(SocketMgr.NetConnectedEvent, null);

            this.isConnected = true;
            this.recvThread = new Thread(new ThreadStart(this.ThreadRecvWorker));
            this.recvThread.Start();

            Debug.Log("connect to server success" + this.serverIp + ":" + this.port + "!");
        }
        catch (System.Exception e) {
            Debug.Log(e.ToString());
            this.OnConnectError(e.ToString());
            this.isConnected = false;
            this.state = State.Disconnect;
            EventMgr.Instance.Emit(SocketMgr.NetDisconnectEvent, null);
        }
    }

    public void Close() {
        if (!this.isConnected) {
            return;
        }

        this.isConnected = false;
        this.state = State.Disconnect;
        EventMgr.Instance.Emit(SocketMgr.NetDisconnectEvent, 1);

        // abort recv thread
        if (this.recvThread != null) {
            this.recvThread.Interrupt();
            this.recvThread.Abort();

            this.recvThread.Join();
            this.recvThread = null;
        }
        // end

        if (this.clientSocket != null && this.clientSocket.Connected) {
            this.clientSocket.Close();
            this.clientSocket = null;
        }

        // 等待线程结束
        // end
    }
    
    private void OnSendDataEnded(IAsyncResult iar)
    {
        try
        {
            Socket client = (Socket)iar.AsyncState;
            client.EndSend(iar);
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    private void SendCmd(byte[] cmdData) {
        
        if (cmdData == null) {
            return;
        }

        byte[]tcpPkg = TcpPacker.pack(cmdData);

        // end 
        this.clientSocket.BeginSend(tcpPkg, 0, tcpPkg.Length, SocketFlags.None, new AsyncCallback(this.OnSendDataEnded), this.clientSocket);
        // end 
    }

    // [2, 2, 4, body]
    public void SendMsg(Message msg, short machineId, long utag = 0) {
        if (this.state != State.Connected) {
            return;
        }

        var msgBody = SerializerHelper.PBEncoder(msg);
        if (msgBody == null) {
            return;
        }

        byte[] body = new byte[HEAD_SIZE + msgBody.Length];

        short module = msg.GetModule();
        module = (short)(((int)module) | (machineId << 8));

        DataViewer.WriteShortLE(body, 0, module);
        DataViewer.WriteShortLE(body, 2, msg.GetCmd());
        DataViewer.WriteULongLE(body, 4, (ulong)utag);
        DataViewer.WriteBytes(body, HEAD_SIZE, msgBody);

        this.SendCmd(body);
    }
}
