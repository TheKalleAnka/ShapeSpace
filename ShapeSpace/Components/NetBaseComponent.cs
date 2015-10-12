using Lidgren.Network;

abstract class NetBaseComponent /*: BaseComponent?*/
{
    protected NetPeer peer;

    /// <summary>
    /// Instantiates the base peer which can be cast
    /// to either client or server in derived classes
    /// </summary>
    /// <param name="config">The configuration file for the peer</param>
    public NetBaseComponent(NetPeerConfiguration config)
    {
        peer = new NetPeer(config);
    }

    /// <summary>
    /// Move to a base component?
    /// </summary>
    public virtual void Update() { }

    public virtual void Draw() { }
}