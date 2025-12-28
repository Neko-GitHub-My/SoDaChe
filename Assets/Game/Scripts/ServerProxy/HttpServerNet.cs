using Framework.Core.Serializer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HttpServerNet : Singleton<HttpServerNet>
{
    private string serverUrl = "http://127.0.0.1:6090/";
    public void SetServerUri(string uri = null) {
        if (uri != null) {
            this.serverUrl = uri;
        }
    }

    private const int HEAD_SIZE = 14;

    private Message DecodeMessage(byte[] data)
    {

        short module = DataViewer.ReadShortLE(data, 0); // LEµÄ·½Ê½
        module = (short)(module & 0x00ff);
        short cmd = DataViewer.ReadShortLE(data, 2);

        int count = data.Length;
        Message msg = SerializerHelper.PbDecode(module, cmd, data, HEAD_SIZE, count - HEAD_SIZE);
        return msg;
    }

    public void SendMsg(Message msg, short machineId)
    {
        // Debug.Log(GM_DataMgr.Instance.OpenId);
        byte[] data = MessagePusher.PackMessage(msg, machineId, GM_DataMgr.Instance.OpenId);
        if (data == null) {
            return;
        }

        
        HttpUtils.Post($"{this.serverUrl}{msg.GetType().Name}", null, data, (string err, object body) => {

            if (err != null)
            {
                return;
            }
            Message msg = this.DecodeMessage(body as byte[]);
            if (msg != null)
            {
                MessageDispatcher.Instance.OnRecvServerResponse(msg);
            }
        });
    }
}
