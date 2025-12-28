
using Framework.Core.Serializer;

public class MessagePusher {
    private const int HEAD_SIZE = 14; // [2, 2, 8, 2]

    public static byte[] PackMessage(Message m, int machineId, long openId) {
        byte[] msgBody = SerializerHelper.PBEncoder(m);
        if (msgBody == null) {
            return null;
        }


        byte[] body = new byte[HEAD_SIZE + msgBody.Length];
        short module = m.GetModule();
        module = (short)(((int)module) | (machineId << 8));

        DataViewer.WriteShortLE(body, 0, module);
        DataViewer.WriteShortLE(body, 2, m.GetCmd());
        DataViewer.WriteULongLE(body, 4, (ulong)openId);
        DataViewer.WriteBytes(body, HEAD_SIZE, msgBody);

        return body;
    }
}


