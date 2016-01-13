using System.Windows.Forms;
using FarseerPhysics.Dynamics;
using Lidgren.Network;
using Lidgren.Network.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShapeSpace.Network;
using ShapeSpaceHelper;
using System.Collections.Generic;

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
    //When this reaches the desired value, an input package will be sent to the server
    float sendTimer = 0;
    //Number of times every second that the game will send the current inputs to the server
    const float sentInputPackagesPerSecond = 20;

    //Contains all the movement inputs that have been registered since last sending an input package
    List<InputWithTime> inputsPendingDeparture = new List<InputWithTime>();
    //Keeps check of the time since an input was last added to the pending inputs list
    float timeSinceLastAddedInput = 0;

    public GameComponent(GraphicsDevice graphicsDevice) : base(graphicsDevice) 
    {
        camera = new Camera(graphicsDevice.Viewport);//Use for spritebatch.begin
    }

    public void Initialize()
    {
        physWorld = new World(new Vector2(0,0));
        player = new Player(spriteBatch.GraphicsDevice, new Vector2(0,0), physWorld);
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
        //Handle inputs
        Vector2 vector = InputManager.GetMovementInputAsVector();
        /*
        timeSinceLastAddedInput += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if(vector != inputsPendingDeparture[inputsPendingDeparture.Count - 1].Input)
        {
            inputsPendingDeparture.Add(new InputWithTime(timeSinceLastAddedInput, vector));
        }

        if ((sendTimer += (float)gameTime.ElapsedGameTime.TotalSeconds) >= 1 / sentInputPackagesPerSecond)
        {
            //SEND THE INPUTS TO THE SERVER
        }
        */
        physWorld.Step((float)gameTime.ElapsedGameTime.TotalSeconds);

        if(player != null)
            player.Update(gameTime);

        //camera.Position = player.GetPosition() - camera.Origin + new Vector2(player.scale/2f,player.scale/2f);
    }

    /// <summary>
    /// Draws the player
    /// </summary>
    /// <param name="gameTime"></param>
    public void Draw(GameTime gameTime)
    {
        spriteBatch.Begin(transformMatrix: camera.GetViewMatrix());

        if(player != null)
            player.Draw(ref spriteBatch);

        spriteBatch.End();
    }

    /// <summary>
    /// Connects to the server
    /// </summary>
    /// <param name="ip">The IP of the server</param>
    public void ConnectToServer(string ip)
    {
        NetPeerConfiguration config = new NetPeerConfiguration("ShapeSpace");
        config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);

        client = new NetClient(config);
        client.Start();
        //Should be changed to other way of handling
        client.RegisterReceivedCallback(HandleClientMessages);
        client.Connect(ip,55678);
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

    void SendMessageToServer(ShapeSpace.Network.ShapeCustomNetMessageType type)
    {
        NetOutgoingMessage outmessage = client.CreateMessage();

        switch(type)
        {
            case ShapeCustomNetMessageType.InputUpdate:
                outmessage.Write((byte)ShapeCustomNetMessageType.InputUpdate);

                outmessage.Write(inputsPendingDeparture.Count);
                foreach(InputWithTime item in inputsPendingDeparture)
                {
                    outmessage.Write(item.TimeSincePrevious);
                    outmessage.Write(item.Input);
                }
                break;
        }
    }

    public void UpdateGameState(GameStates newGameState)
    {
        gameState = newGameState;
    }
}
