using Framework.Core.Serializer;
using System;
using System.IO;
using UnityEngine;



public class SerializerHelper {
        
    static public byte[] PBEncoder(Message msg) {
        byte[] body = null;

        try {
            using (var stream = new MemoryStream()) {
                ProtoBuf.Serializer.Serialize(stream, msg);
                body = stream.ToArray();
            }
        }
        catch (IOException e) {
            Debug.Log(e.Message);
        }
        return body;
    }

    static public Message PbDecode(short module, short cmd, byte[] body, int offset, int count) {

        MessageFactory.Instance.GetMessage(module, cmd, out Type msgType);

        if (body == null) {
            return (Message)Activator.CreateInstance(msgType);
        }

        try {
            using (var stream = new MemoryStream(body, offset, count)) {
                Message _fw = (Message)ProtoBuf.Serializer.Deserialize(msgType, stream);
                return _fw;
            }
        } catch (Exception e) {
            Debug.LogError($"读取消息出错,模块号[{module}]，类型[{cmd}],异常:{e.Message}");
        }

        return null;
    }
}
