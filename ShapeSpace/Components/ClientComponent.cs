using Lidgren.Network;

class ClientComponent : NetBaseComponent
{
    NetClient client;

    public ClientComponent(NetPeerConfiguration config) : base(config) 
    {
        client = (NetClient)base.peer;
    }

    public override void Update() { }
    public void ConnectToServer() { }
    public void SendMessageToServer() { }
    void HandleMessageFromServer() { }
}
