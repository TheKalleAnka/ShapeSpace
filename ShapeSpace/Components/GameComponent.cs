using System.Windows.Forms;
using FarseerPhysics.Dynamics;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// The component that handles all things related to gameplay
/// </summary>
class GameComponent : BaseComponent, IDrawable, IUpdateable, ILoadable, IInitializable
{
    //GAMEPLAY
    Player player;
    
    //PHYSICS
    World physWorld;
    
    //NETWORK
    NetClient client;

    public GameComponent(GraphicsDevice graphicsDevice) : base(graphicsDevice) 
    {
        camera = new Camera(graphicsDevice.Viewport);//Use for spritebatch.begin
    }

    public void Initialize()
    {
        physWorld = new World(new Vector2(0,0));
        player = new Player(true, false, 1, spriteBatch.GraphicsDevice, new Vector2(0,0), physWorld);
    }

    SpriteFont font;

    public void LoadContent(ContentManager cManager)
    {
        player.LoadContent(cManager);
        font = cManager.Load<SpriteFont>("text");
    }

    public void UnloadContent()
    {
        player.UnloadContent();
    }

    public void Update(GameTime gameTime)
    {
        physWorld.Step((float)gameTime.ElapsedGameTime.TotalSeconds);

        if(player != null)
            player.Update(gameTime);

        camera.Position = player.GetPosition() - camera.Origin + new Vector2(player.scale/2f,player.scale/2f);
        
    }

    public void Draw(GameTime gameTime)
    {
        spriteBatch.Begin(transformMatrix: camera.GetViewMatrix());

        if(player != null)
            player.Draw(ref spriteBatch);

        spriteBatch.End();
    }

    public void ConnectToServer()
    {
        NetPeerConfiguration config = new NetPeerConfiguration("ShapeSpace");
        config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);

        client = new NetClient(config);
        client.Start();
        client.RegisterReceivedCallback(HandleClientMessages);
        client.Connect("127.0.0.1",55678);
    }

    void HandleClientMessages(object peer)
    {
        NetIncomingMessage msg;
        while((msg = client.ReadMessage()) != null)
        {
            switch (msg.MessageType)
            {
                case NetIncomingMessageType.DiscoveryResponse:
                    MessageBox.Show(msg.ReadString() + " | " + msg.ReadIPEndPoint() + " | " + msg.SenderEndPoint);
                     
                    if(client.GetConnection(msg.SenderEndPoint) == null)
                        client.Connect(msg.SenderEndPoint);
                    break;
                default:
                    //Console.WriteLine("Unhandled type: " + msg.MessageType);
                    break;
            }
            client.Recycle(msg);
        }
    }
}
