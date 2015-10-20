using Lidgren.Network;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

abstract class NetBaseComponent
{
    SpriteBatch spriteBatch;




    protected NetPeer peer;

    //PHYSICS
    World physicsWorld = new World(new Vector2(0,10));

    Player[] players;

    /// <summary>
    /// Instantiates the base peer which can be cast
    /// to either client or server in derived classes
    /// </summary>
    /// <param name="config">The configuration file for the peer</param>
    public NetBaseComponent(NetPeerConfiguration config, GraphicsDevice graphicsDevice)
    {
        peer = new NetPeer(config);

        

        players = new Player[config.MaximumConnections];
        players[0] = new Player(true,false,1,graphicsDevice,new Vector2(100,100),physicsWorld);
    }

    /// <summary>
    /// Move to a base component?
    /// </summary>
    public virtual void LoadContent() 
    {
        for (int i = 0; i < peer.Configuration.MaximumConnections; i++)
        {
            if (players[i] != null)
                players[i].LoadContent();
        }
    }
    public virtual void Update() { }
    public virtual void Draw(ref SpriteBatch spriteBatch, GameTime gameTime)
    {
        physicsWorld.Step((float)gameTime.ElapsedGameTime.TotalSeconds);

        for (int i = 0; i < peer.Configuration.MaximumConnections; i++)
        {
            if (players[i] != null)
                players[i].Draw(ref spriteBatch);
        }
    }
}