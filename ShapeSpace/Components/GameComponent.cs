using System.Collections.Generic;
using System.Windows.Forms;
using Lidgren.Network;
using Lidgren.Network.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShapeSpace.Gameplay;
using ShapeSpace.Network;

/// <summary>
/// The component that handles all things related to gameplay
/// </summary>
class GameComponent : BaseComponent, IDrawable, IUpdateable, ILoadable, IInitializable
{
    //GAMEPLAY
    Player player;
    //A mirror of the array on the server and contains the player version of those on there
    Player[] playersOnSameServer;
    List<CollisionRemnant> remnants = new List<CollisionRemnant>();

    //DRAWING
    ContentManager cManager;
    
    //NETWORK
    NetClient client;
    //When this reaches the desired value, an input package will be sent to the server
    float lastSentInput = 0;
    //Number of times every second that the game will send the current inputs to the server
    const float sentInputPackagesPerSecond = 20;

    public GameComponent(GraphicsDevice graphicsDevice) : base(graphicsDevice) 
    {
        camera = new Camera(graphicsDevice.Viewport);//Use for spritebatch.begin
    }

    public void Initialize()
    {
        player = new Player(spriteBatch.GraphicsDevice);

        NetPeerConfiguration config = new NetPeerConfiguration("ShapeSpace");
        config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
        client = new NetClient(config);
        client.Start();
        //The received messages should be handled in a message loop in update #ThisIsTemporary
        client.RegisterReceivedCallback(HandleClientMessages);
    }

    //The font used when writing things on the screen
    SpriteFont font;

    /// <summary>
    /// Called by the shell to load any necessary content
    /// </summary>
    /// <param name="cManager">The ContentManager reponsible for loading the content</param>
    public void LoadContent(ContentManager cManager)
    {
        this.cManager = cManager;

        player.LoadContent(cManager);
        font = cManager.Load<SpriteFont>("text");
    }

    /// <summary>
    /// Called by the shell when the game is closing to release resources
    /// </summary>
    public void UnloadContent()
    {
        player.UnloadContent();
    }

    public void Update(GameTime gameTime)
    {
        //Handle inputs
        Vector2 vector = InputManager.GetMovementInputAsVector();
        //UIComponent.Instance._DebugString = vector.ToString();

        lastSentInput += (float)gameTime.ElapsedGameTime.TotalSeconds;

        //If it is time to send a package of inputs
        if(lastSentInput >= 1f/sentInputPackagesPerSecond && player.indexOnServer >= 0)
        {
            NetOutgoingMessage outMessage = client.CreateMessage();
            outMessage.Write((byte)ShapeCustomNetMessageType.InputUpdate);
            outMessage.Write(player.indexOnServer);
            outMessage.Write(lastSentInput);
            outMessage.Write(InputManager.GetMovementInputAsVector());
            client.SendMessage(outMessage, NetDeliveryMethod.UnreliableSequenced, 1);

            lastSentInput = 0;
        }

        //Update the local player
        if(player != null)
            player.Update(gameTime);

        //Update the other players
        if(playersOnSameServer != null)
            for (int i = 0; i < playersOnSameServer.Length; i++)
            {
                if (playersOnSameServer[i] != null)
                    playersOnSameServer[i].Update(gameTime);
            }

        //Set the camera to the players position when the player is created and don't move it afterward
        if (!tempTest && player != null)
        {
            camera.Position = player.positionNow - camera.Origin + new Vector2(player.power / 2f, player.power / 2f);
            tempTest = true;
        }
        //UIComponent.Instance._DebugString
    }

    bool tempTest = false;

    /// <summary>
    /// Draws the player
    /// </summary>
    /// <param name="gameTime"></param>
    public void Draw(GameTime gameTime)
    {
        if (playersOnSameServer != null)
        {
            spriteBatch.Begin(transformMatrix: camera.GetViewMatrix());

            for (int i = 0; i < remnants.Count; i++)
            {
                remnants[i].Draw(ref spriteBatch);
            }

            if (player != null)
                player.Draw(ref spriteBatch);

            for (int i = 0; i < playersOnSameServer.Length; i++)
            {
                if (playersOnSameServer[i] != null)
                    if (playersOnSameServer[i].indexOnServer != player.indexOnServer)
                        playersOnSameServer[i].Draw(ref spriteBatch);
            }

            spriteBatch.End();
        }
    }

    /// <summary>
    /// Connects to the server
    /// </summary>
    /// <param name="ip">The IP of the server</param>
    public void ConnectToServer(string ip, int port)
    {
        client.Connect(ip,port);
    }

    public void DiscoverLocalServers()
    {
        client.DiscoverLocalPeers(55678);
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
                                float powr = msg.ReadFloat();
                                float time = msg.ReadFloat();
                                Vector2 pos = msg.ReadVector2();

                                if (index == player.indexOnServer)
                                {
                                    player.power = powr;
                                    player.positions.Add(new PositionInTime(time, pos, false));
                                }
                                else if (playersOnSameServer[index] != null)
                                {
                                    playersOnSameServer[index].power = powr;
                                    playersOnSameServer[index].positions.Add(new PositionInTime(time, pos, false));
                                }

                                int numTrail = msg.ReadInt32();

                                for(int j = 0; j < numTrail; j++)
                                {
                                    Vector2 position = msg.ReadVector2();
                                    float size = msg.ReadFloat();

                                    if(index == player.indexOnServer)
                                    {
                                        if(j < player.trail.Count)
                                        {
                                            if (player.trail[j] != null)
                                            {
                                                player.trail[j].position = position;
                                                player.trail[j].size = size;
                                            }

                                            if (numTrail < player.trail.Count)
                                                player.trail.RemoveRange(numTrail - 1,player.trail.Count - numTrail);
                                        }
                                        else
                                        {
                                            Trail t = new Trail(position, size, Color.Beige, spriteBatch.GraphicsDevice, null);
                                            player.trail.Add(t);
                                        }
                                    }
                                    else if (playersOnSameServer[index] != null)
                                    {
                                        if(j < playersOnSameServer[index].trail.Count)
                                        {
                                            if (playersOnSameServer[index].trail[j] != null)
                                            {
                                                playersOnSameServer[index].trail[j].position = position;
                                                playersOnSameServer[index].trail[j].size = size;
                                            }

                                            if (numTrail < playersOnSameServer[index].trail.Count)
                                                playersOnSameServer[index].trail.RemoveRange(numTrail - 1, playersOnSameServer[index].trail.Count - numTrail);
                                        }
                                        else
                                        {
                                            Trail t = new Trail(position, size, Color.Beige, spriteBatch.GraphicsDevice, null);
                                            playersOnSameServer[index].trail.Add(t);
                                        }
                                    }
                                }
                            }

                            int numRemnants = msg.ReadInt32();

                            for (int i = 0; i < numRemnants; i++ )
                            {
                                Vector2 pos = msg.ReadVector2();
                                float size = msg.ReadFloat();

                                if(i < remnants.Count)
                                {
                                    remnants[i].position = pos;
                                    remnants[i].size = size;
                                }
                                else
                                {
                                    CollisionRemnant newRemnant = new CollisionRemnant(pos, size, Color.GhostWhite, spriteBatch.GraphicsDevice, null);
                                    remnants.Add(newRemnant);
                                }

                                if (numRemnants < remnants.Count)
                                    remnants.RemoveRange(numRemnants - 1, remnants.Count - numRemnants);
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
                            player.SetTeam((ShapeTeam)msg.ReadByte());
                            
                            int numOfPlayers1 = msg.ReadInt32();

                            for (int i = 0; i < numOfPlayers1; i++ )
                            {
                                Player p = new Player(spriteBatch.GraphicsDevice);
                                int index = msg.ReadInt32();
                                p.indexOnServer = index;
                                p.SetTeam((ShapeTeam)msg.ReadByte());

                                int powaar = msg.ReadInt32();
                                //p.power = powaar;

                                p.LoadContent(cManager);

                                playersOnSameServer[index] = p;
                            }
                            break;
                        case ShapeCustomNetMessageType.SetupFailed:
                            MessageBox.Show(msg.ReadString());
                            break;
                    }
                    break;
                case NetIncomingMessageType.DiscoveryResponse:
                    //MessageBox.Show(msg.ReadString() + " | " + msg.ReadIPEndPoint() + " | " + msg.SenderEndPoint);

                    if (client.GetConnection(msg.SenderEndPoint) == null)
                        ConnectToServer(msg.SenderEndPoint.Address.ToString(),55678);
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
                            //outMsg.Write((byte)ShapeTeam.BLUE);
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