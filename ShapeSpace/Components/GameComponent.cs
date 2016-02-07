using System.Windows.Forms;
using Lidgren.Network;
using Lidgren.Network.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShapeSpace.Network;
using System.Collections.Generic;

/// <summary>
/// The component that handles all things related to gameplay
/// </summary>
class GameComponent : BaseComponent, IDrawable, IUpdateable, ILoadable, IInitializable
{
    //GAMEPLAY
    Player player;
    
    //PHYSICS
    
    //NETWORK
    NetClient client;
    //When this reaches the desired value, an input package will be sent to the server
    float lastSentInput = 0;
    //Number of times every second that the game will send the current inputs to the server
    const float sentInputPackagesPerSecond = 50;

    public GameComponent(GraphicsDevice graphicsDevice) : base(graphicsDevice) 
    {
        camera = new Camera(graphicsDevice.Viewport);//Use for spritebatch.begin
    }

    public void Initialize()
    {
        player = new Player(spriteBatch.GraphicsDevice);
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
        UIComponent.Instance._DebugString = vector.ToString();

        lastSentInput += (float)gameTime.ElapsedGameTime.TotalSeconds;

        //If it is time to send a package of inputs
        if(lastSentInput >= 1f/sentInputPackagesPerSecond && client != null)
        {
            NetOutgoingMessage outMessage = client.CreateMessage();
            outMessage.Write((byte)ShapeCustomNetMessageType.InputUpdate);
            outMessage.Write(lastSentInput);
            outMessage.Write(InputManager.GetMovementInputAsVector());
            client.SendMessage(outMessage, NetDeliveryMethod.ReliableOrdered);

            lastSentInput = 0;
        }

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
        //The received messages should be handled in a message loop in update #ThisIsTemporary
        client.RegisterReceivedCallback(HandleClientMessages);
        client.Connect(ip,55678);
    }

    void HandleClientMessages(object peer)
    {
        NetIncomingMessage msg;
        while ((msg = client.ReadMessage()) != null)
        {
            switch (msg.MessageType)
            {
                case NetIncomingMessageType.Data:
                    switch((ShapeCustomNetMessageType)msg.ReadByte())
                    {
                        case ShapeCustomNetMessageType.LocationUpdate:
                            float time = msg.ReadFloat();
                            Vector2 pos = msg.ReadVector2();

                            player.positions.Add(new PositionInTime(time,pos));

                            player.positionNow = pos;

                            break;
                    }
                    break;
                case NetIncomingMessageType.DiscoveryResponse:
                    MessageBox.Show(msg.ReadString() + " | " + msg.ReadIPEndPoint() + " | " + msg.SenderEndPoint);

                    if (client.GetConnection(msg.SenderEndPoint) == null)
                        client.Connect(msg.SenderEndPoint);
                    break;
                case NetIncomingMessageType.StatusChanged:
                    switch ((NetConnectionStatus)msg.ReadByte())
                    {
                        //When connected to the server
                        case NetConnectionStatus.Connected:
                            //1. Handle hailmessage containing server info
                            //To be implemented

                            //2. Send client info
                            NetOutgoingMessage outMsg = client.CreateMessage();
                            outMsg.Write((byte)ShapeCustomNetMessageType.SetupRequest);
                            outMsg.Write((byte)ShapeTeam.BLUE);
                            outMsg.Write("UserName");
                            client.SendMessage(outMsg, NetDeliveryMethod.ReliableOrdered);
                            break;
                        //When disconnected from the server
                        case NetConnectionStatus.Disconnected:
                            //Contains a string of the reason for the disconnection
                            string reason = msg.ReadString();
                            if (string.IsNullOrEmpty(reason))
                                MessageBox.Show("Disconnected! Reason unknown.");
                            else
                                MessageBox.Show("Disconnected, Reason: " + reason);
                            break;
                    }
                    break;
                default:
                    //Console.WriteLine("Unhandled type: " + msg.MessageType);
                    break;
            }
            client.Recycle(msg);
        }
    }

    void SendMessageToServer(string s)
    {
        NetOutgoingMessage outmessage = client.CreateMessage();
        outmessage.Write(s);
        client.SendMessage(outmessage, NetDeliveryMethod.ReliableOrdered);
    }

    public void UpdateGameState(GameStates newGameState)
    {
        gameState = newGameState;
    }
}
