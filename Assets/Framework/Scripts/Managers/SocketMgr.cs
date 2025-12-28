using Framework.Core.Serializer;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class SocketMgr : UnitySingleton<SocketMgr>
{
    public const int NetRecvCmdEvent = 0x01;
    public const int NetDisconnectEvent = 0x02;
    public const int NetConnectedEvent = 0x3;
    public const int NetConnectingEvent = 0x4;

    private ISocketWrapper socketWrapper = null;

    public void InitTcpSocket(string ip, int port) {
        if (this.socketWrapper != null) {
            this.socketWrapper.Close();
            this.socketWrapper = null;
        }

        TcpSocketWrapper tcp = new TcpSocketWrapper();
        tcp.Init(ip, port);

        this.socketWrapper = tcp;

    }

    public void InitWebSocket(string url) {
        if (this.socketWrapper != null) {
            this.socketWrapper.Close();
            this.socketWrapper = null;
        }

        WebSocketWrapper ws = new WebSocketWrapper();
        ws.Init(url);

        this.socketWrapper = ws;
    }

    public void ConnectToServer() {
        this.socketWrapper.ConnectToServer();
    }

    public void SendMsg(Message msg, short machineId, long utag = 0) {
        this.socketWrapper.SendMsg(msg, machineId, utag);
    }

    public void Close() {
        if (this.socketWrapper != null) {
            this.socketWrapper.Close();
            this.socketWrapper = null;
        }
    }

    void OnDestroy()
    {
        Debug.Log("OnDestroy###########");
        this.Close();
    }

    void OnApplicaitonQuit()
    {
        Debug.Log("OnApplicaitonQuit###########");
        this.Close();
    }

    void Update()
    {
        if (this.socketWrapper != null) {
            this.socketWrapper.Update();
        }
    }

}
