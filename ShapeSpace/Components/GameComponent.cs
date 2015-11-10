using System;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Lidgren.Network;
using System.Threading;
using System.Windows.Forms;

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
    NetServer server;
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

        if (server != null)
            spriteBatch.DrawString(font,"Connections: " + server.Connections.Count,new Vector2(-1200,-800),Color.White,0,new Vector2(0,0),10,SpriteEffects.None,0);

        spriteBatch.End();
    }

    /// <summary>
    /// Starts a new server
    /// </summary>
    /// <param name="maxConnect">The maximum amount of clients that can join the server</param>
    public void StartServer(int maxConnect)
    {
        NetPeerConfiguration config = new NetPeerConfiguration("ShapeSpace");
        config.Port = 55678;
        config.MaximumConnections = maxConnect;
        config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);

        bool serverStarted = true;

        try
        {
            server = new NetServer(config);
            server.Start();
        }
        catch(Exception e)
        {
            serverStarted = false;
        }

        if (serverStarted)
        {
            Thread t = new Thread(HandleServerMessages);
            t.IsBackground = true;
            t.Start();
        }
    }

    public void ConnectToServer()
    {
        NetPeerConfiguration config = new NetPeerConfiguration("ShapeSpace");
        config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);

        client = new NetClient(config);
        client.Start();
        client.DiscoverLocalPeers(55678);

        Thread t = new Thread(HandleClientMessages);
        t.IsBackground = true;
        t.Start();
    }

    void HandleClientMessages()
    {
        NetIncomingMessage msg;
        while (client != null)
        {
            while((msg = client.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.DiscoveryResponse:
                        MessageBox.Show(msg.ReadString() + " | " + msg.ReadIPEndPoint() + " | " + msg.SenderEndPoint);
                        
                        client.Connect(msg.SenderEndPoint);
                        break;
                    default:
                        //Console.WriteLine("Unhandled type: " + msg.MessageType);
                        break;
                }
                client.Recycle(msg);
                msg = null;
            }
        }
    }

    void HandleServerMessages()
    {
        while (true)
        {
            NetIncomingMessage msg;
            while ((msg = server.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.DiscoveryRequest:

                        // Create a response and write some example data to it
                        NetOutgoingMessage response = server.CreateMessage();
                        response.Write("This is a great server name!");
                        response.Write(msg.SenderEndPoint);

                        // Send the response to the sender of the request
                        server.SendDiscoveryResponse(response, msg.SenderEndPoint);
                        break;
                    default:
                        //Console.WriteLine("Unhandled type: " + msg.MessageType);
                        break;
                }
                server.Recycle(msg);
                msg = null;
            }
        }
    }
}
