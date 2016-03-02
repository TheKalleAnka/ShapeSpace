using System.Windows.Forms;
using Lidgren.Network;
using Lidgren.Network.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShapeSpace.Network;

/// <summary>
/// The component that handles all things related to gameplay
/// </summary>
class GameComponent : BaseComponent, IDrawable, IUpdateable, ILoadable, IInitializable
{
    //GAMEPLAY
    Player player;
    Player[] playersOnSameServer;

    //DRAWING
    ContentManager cManager;
    
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
        this.cManager = cManager;

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
        if(lastSentInput >= 1f/sentInputPackagesPerSecond && player.indexOnServer >= 0)
        {
            NetOutgoingMessage outMessage = client.CreateMessage();
            outMessage.Write((byte)ShapeCustomNetMessageType.InputUpdate);
            outMessage.Write(player.indexOnServer);
            outMessage.Write(lastSentInput);
            outMessage.Write(InputManager.GetMovementInputAsVector());
            client.SendMessage(outMessage, NetDeliveryMethod.UnreliableSequenced);

            lastSentInput = 0;
        }

        if(player != null)
            player.Update(gameTime);

        camera.Position = player.positionNow - camera.Origin + new Vector2(player.power/2f,player.power/2f);
    }

    /// <summary>
    /// Draws the player
    /// </summary>
    /// <param name="gameTime"></param>
    public void Draw(GameTime gameTime)
    {
        if(playersOnSameServer != null)
        {
            spriteBatch.Begin(transformMatrix: camera.GetViewMatrix());

            if (player != null)
                player.Draw(ref spriteBatch);
            
            for (int i = 0; i < playersOnSameServer.Length; i++)
            {
                if (playersOnSameServer[i] != null)
                    playersOnSameServer[i].Draw(ref spriteBatch);
            }

            spriteBatch.End();
        }
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
                        //Contains the current locations of the players on the server
                        case ShapeCustomNetMessageType.LocationUpdate:
                            int numOfPlayers = msg.ReadInt32();

                            for (int i = 0; i < numOfPlayers; i++)
                            {
                                int index = msg.ReadInt32();
                                float time = msg.ReadFloat();
                                Vector2 pos = msg.ReadVector2();

                                if (index == player.indexOnServer)
                                {
                                    player.positions.Add(new PositionInTime(time, pos));
                                    player.positionNow = pos;
                                }
                                else if (playersOnSameServer[index] != null)
                                {
                                    //playersOnSameServer[i].positions.Add(new PositionInTime(time, pos));
                                    playersOnSameServer[i].positionNow = pos;
                                }
                            }
                            break;
                        //A new player has joined the server which has to be added to this client
                        case ShapeCustomNetMessageType.NewPlayerJoined:
                            int indexOnServer = msg.ReadInt32();
                            ShapeTeam team = (ShapeTeam)msg.ReadByte();
                            int power = msg.ReadInt32();

                            Player newPlayer = new Player(spriteBatch.GraphicsDevice);

                            newPlayer.indexOnServer = indexOnServer;
                            newPlayer.SetTeam(team);
                            newPlayer.power = power;

                            newPlayer.LoadContent(cManager);

                            playersOnSameServer[indexOnServer] = newPlayer;
                            
                            //MessageBox.Show("New player jioned");
                            break;
                        case ShapeCustomNetMessageType.SetupSuccessful:
                            //Have our index on server handed to us
                            player.indexOnServer = msg.ReadInt32();
                            
                            int numOfPlayers1 = msg.ReadInt32();

                            for (int i = 0; i < numOfPlayers1; i++ )
                            {
                                Player p = new Player(spriteBatch.GraphicsDevice);
                                int index = msg.ReadInt32();
                                p.indexOnServer = index;
                                p.SetTeam((ShapeTeam)msg.ReadByte());
                                p.power = msg.ReadInt32();

                                p.LoadContent(cManager);

                                playersOnSameServer[index] = p;
                            }
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
                            int maxPlayersOnServer = msg.SenderConnection.RemoteHailMessage.ReadInt32();
                            playersOnSameServer = new Player[maxPlayersOnServer];

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

                            HandleDisconnection();
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

    void HandleDisconnection()
    {
        playersOnSameServer = null;

        UIComponent.Instance.ShowMainMenu();
    }

    public void UpdateGameState(GameStates newGameState)
    {
        gameState = newGameState;
    }
}