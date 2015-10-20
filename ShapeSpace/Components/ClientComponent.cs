using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// This is the component used for the client.
/// It handles the drawing, gameplay and physics,
/// as well as sending data to the server.
/// </summary>
class ClientComponent : NetBaseComponent
{
    NetClient client;

    public ClientComponent(NetPeerConfiguration config, GraphicsDevice graphicsDevice) : base(config, graphicsDevice) 
    {
        //client = (NetClient)base.peer;
    }

    public override void LoadContent()
    {
        base.LoadContent();
    }
    public override void Update() { base.Update(); }
    public override void Draw(ref SpriteBatch spriteBatch, GameTime gameTime) { base.Draw(ref spriteBatch, gameTime); }
    public void ConnectToServer() { }
    public void SendMessageToServer() { }
    void HandleMessageFromServer() { }
}
