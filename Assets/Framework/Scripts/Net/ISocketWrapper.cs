using Framework.Core.Serializer;



public enum State
{
    Invalid = -1,
    Disconnect = 0,
    Connecting = 1,
    Connected = 2,
}

public interface ISocketWrapper
{
    public void ConnectToServer();
    public void Close();
    public void Update();
    public void SendMsg(Message msg, short machineId, long utag = 0);


}

