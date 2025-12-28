using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class ServerZoneInfo
{
    public int socketType = -1; // 0 Tcp, 1 WebSocket, 2 http;
    public string serverName;
    public string serverIp;
    public int port;

    public string websitUri = null;
    public int useDirectConn = 0;
}

public class DirectConnInfo {
    public string serverIp;
    public int tcpPort;
    public int wsPort;
    public int httpPort;
}

public class ServerZoneMgr : Singleton<ServerZoneMgr>
{
    // private string httpServerZoneUrl = "http://127.0.0.1:16090/PullingServerZone";
    private string httpServerZoneUrl = "http://127.0.0.1:6090/PullingServerZone"; 
    private List<ServerZoneInfo> serverZoneInfo = null;
    private Dictionary<int, DirectConnInfo> directConnInfo = null;
    private int socketType = -1;
    private int prevMachineId = -1;

    public async Task SetConnectServerZone(ServerZoneInfo serverZone) {
        if (serverZone.websitUri != null) {
            HttpServerNet.Instance.SetServerUri(serverZone.websitUri);
            if (serverZone.useDirectConn == 1) {
                AuthProxy.Instance.useHttp = true;
                PlayerProxy.Instance.useHttp = true;
                await this.PullServerDirectConnConfig(serverZone.websitUri);
            }
        }
        this.socketType = serverZone.socketType;
        GM_DataMgr.Instance.useDirectConn = serverZone.useDirectConn;

        if (serverZone.useDirectConn == 1) {
            return;
        }

        AuthProxy.Instance.useHttp = false;
        PlayerProxy.Instance.useHttp = false;

        if (serverZone.socketType == 0) {
            SocketMgr.Instance.InitTcpSocket(serverZone.serverIp, serverZone.port);
            SocketMgr.Instance.ConnectToServer();
        }
        else if (serverZone.socketType == 1) {
            SocketMgr.Instance.InitWebSocket($"ws://{serverZone.serverIp}:{serverZone.port}/ws");
            SocketMgr.Instance.ConnectToServer();
        }
    }

    public void DirectConnectToMachineServer(int machineId) {
        if (this.prevMachineId == machineId) {
            return;
        }

        this.prevMachineId = machineId;

        DirectConnInfo info = this.directConnInfo[machineId];
        
        if (info == null) {
            Debug.Log("null machine");
            return;
        }

        Debug.Log(info.serverIp + ":" + info.tcpPort + ":" + this.socketType);
        if (this.socketType == 0)
        {
            SocketMgr.Instance.InitTcpSocket(info.serverIp, info.tcpPort);
            SocketMgr.Instance.ConnectToServer();
        }
        else if (this.socketType == 1)
        {
            SocketMgr.Instance.InitWebSocket($"ws://{info.serverIp}:{info.wsPort}/ws");
            SocketMgr.Instance.ConnectToServer();
        }
    }

    public async Task PullServerDirectConnConfig(string websitUri)
    {
        /*UnityWebRequest req = UnityWebRequest.Get($"{websitUri}PullingServerDirectConnConfig");
        await req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError("Pull Server Zone Error!!!");
            return;
        }
        string directConnInfoJson = req.downloadHandler.text;
        // Debug.Log(directConnInfoJson);

        JsonData directConn = JsonMapper.ToObject(directConnInfoJson);
        JsonData array = directConn["servers"];
        
        this.directConnInfo = new Dictionary<int, DirectConnInfo>();
        for (int i = 0; i < array.Count; i++)
        {

            JsonData item = array[i];
            DirectConnInfo serverItem = new DirectConnInfo();
            serverItem.serverIp = item["IpAddr"].ToString();
            serverItem.tcpPort = (int)item["TcpPort"];
            serverItem.wsPort = (int)item["WsPort"];
            serverItem.httpPort = (int)item["HttpPort"];

            this.directConnInfo.Add((int)item["ID"], serverItem);
        }*/
    }

    public async Task Init() {
        /*UnityWebRequest req = UnityWebRequest.Get(this.httpServerZoneUrl);
        await req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.ConnectionError) {
            Debug.LogError("Pull Server Zone Error!!!");
            return;
        }
        string serverInfoJson = req.downloadHandler.text;
        // Debug.Log(serverInfoJson);

        JsonData serverZone = JsonMapper.ToObject(serverInfoJson);
        JsonData array = serverZone["ServerZones"];
        if (array.Count <= 0) {
            return;
        }

        this.serverZoneInfo = new List<ServerZoneInfo>();
        for (int i = 0; i < array.Count; i++) {

            JsonData item = array[i];
            ServerZoneInfo serverItem = new ServerZoneInfo();
            serverItem.serverName = item["GameZone"].ToString();
            serverItem.serverIp = item["ServerIp"].ToString();
            serverItem.socketType = 0;
            serverItem.port = (int)item["TcpPort"];
            serverItem.websitUri = item["HttpWebsite"].ToString();
            serverItem.useDirectConn = (int)item["useDirectConn"];
            this.serverZoneInfo.Add(serverItem);

            serverItem = new ServerZoneInfo();
            serverItem.serverName = item["GameZone"].ToString();
            serverItem.serverIp = item["ServerIp"].ToString();
            serverItem.socketType = 1;
            serverItem.port = (int)item["WsPort"];
            serverItem.useDirectConn = (int)item["useDirectConn"];
            serverItem.websitUri = item["HttpWebsite"].ToString();
            this.serverZoneInfo.Add(serverItem);
        }*/
    }

    public List<ServerZoneInfo> GetServerZones() {
        return this.serverZoneInfo;
    }

}
