using Lidgren.Network;

class ServerComponent : NetBaseComponent
{
    NetServer server;

    public ServerComponent(NetPeerConfiguration config) : base(config)
    {
        server = (NetServer)base.peer;
    }

    void StartServer() { }
    void StopServer() { }
    void DisconnectClient() { }
    /// <summary>
    /// Is this needed?
    /// Better would be a method that constructs the whole message and then sends it!
    /// Or several functions that first creates a message, then so you can add to it, then send it.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="recepient"></param>
    /// <param name="deliveryMethod"></param>
    void SendMessage(NetOutgoingMessage msg,NetConnection recepient, NetDeliveryMethod deliveryMethod) { }
}