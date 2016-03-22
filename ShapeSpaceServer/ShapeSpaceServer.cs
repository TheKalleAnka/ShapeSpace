using System;
using Lidgren.Network;
using Lidgren.Network.Xna;
using ShapeSpace.Network;
using ShapeSpace.Gameplay;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Factories;
using System.Threading;
using System.Collections.Generic;
using FarseerPhysics;
using ShapeSpace.Classes;

class Program
{
    static int maxPlayers = 10;

    static NetworkPlayer[] connectedPlayers = new NetworkPlayer[maxPlayers];
    //The number of players that are connected
    static int connectedPlayersActual = 0;

    static NetServer server;
    static World physicsWorld;

    //Remnants
    static List<NetworkCollisionRemnant> remnants = new List<NetworkCollisionRemnant>();

    static void Main(string[] args)
    {
        int loopStartTime = 0;
        int loopEndTime = 0;
        float deltaSecond = 0;

        //Frequency to return data
        const float ReturnDataPerSecond = 20;
        float lastSentData = 0;

        //Team containers
        ShapeTeamContainer greenTeam = new ShapeTeamContainer(ShapeTeam.GREEN);
        ShapeTeamContainer redTeam = new ShapeTeamContainer(ShapeTeam.RED);

        NetPeerConfiguration config = new NetPeerConfiguration("ShapeSpace");
        config.Port = 55678;
        config.MaximumConnections = maxPlayers;
        config.ConnectionTimeout = 10;
        config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
        config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);

        server = new NetServer(config);
        //Console.WriteLine(server.Configuration.LocalAddress);

        physicsWorld = new World(Vector2.Zero);
        //50px = 1m
        ConvertUnits.SetDisplayUnitToSimUnitRatio(50f);

        try
        {
            server.Start();
        }
        catch(Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(e);
            Console.ForegroundColor = ConsoleColor.White;
            WaitForKeyPress();
        }

        Console.WriteLine("Server Started Sucessfully!");

        //Main program loop
        while(true)
        {
            deltaSecond = (loopEndTime - loopStartTime) / 1000f;

            lastSentData += deltaSecond;

            loopStartTime = Environment.TickCount;

            //Handle incoming messages
            NetIncomingMessage msg;
            while ((msg = server.ReadMessage()) != null)
            {
                //Console.WriteLine(msg.MessageType);
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        //A swith handling ShapeSpace custom message types
                        switch((ShapeCustomNetMessageType)msg.ReadByte())
                        {
                            case ShapeCustomNetMessageType.InputUpdate:
                                int playerIndex = msg.ReadInt32();
                                float timeSinceLast = msg.ReadFloat();
                                Vector2 input = msg.ReadVector2();

                                connectedPlayers[playerIndex].inputs.Add(new InputWithTime(timeSinceLast,input));

                                //Console.WriteLine(timeSinceLast + ": " + input.ToString());
                                break;
                            case ShapeCustomNetMessageType.SetupRequest:
                                NetOutgoingMessage returnMessage = server.CreateMessage();

                                NetworkPlayer newPlayer = new NetworkPlayer(physicsWorld, msg.SenderConnection, new Vector2(0,0));

                                //ShapeTeam team = (ShapeTeam)msg.ReadByte();
                                string username = msg.ReadString();

                                try
                                {
                                    //newPlayer.SetTeam(team);
                                    newPlayer.SetUserName(username);

                                    int spot = AddNewPlayer(newPlayer);

                                    newPlayer.indexOnServer = spot;

                                    //Assign the new player to the team with the least amount of players
                                    ShapeTeamContainer newPlayerTeam = greenTeam.GetNumberOfMembers() < redTeam.GetNumberOfMembers() ? greenTeam : redTeam;
                                    bool isBank = newPlayerTeam.AddPlayer(newPlayer.indexOnServer);
                                    newPlayer.SetTeam(newPlayerTeam.GetTeam());
                                    newPlayer.body.Position = ConvertUnits.ToSimUnits(newPlayerTeam.basePosition);

                                    if (isBank)
                                        newPlayer.SetClass(new ShapeClassBank());
                                    else
                                        newPlayer.SetClass(new ShapeClassKnocker());

                                    newPlayer.OnCreateRemnant += CreateRemnant;
                                }
                                catch(Exception e)
                                {
                                    returnMessage.Write((byte)ShapeCustomNetMessageType.SetupFailed);
                                    returnMessage.Write(e.Message);
                                    server.SendMessage(returnMessage, newPlayer.netConnection, NetDeliveryMethod.ReliableUnordered);
                                    break;
                                }
                                
                                returnMessage.Write((byte)ShapeCustomNetMessageType.SetupSuccessful);
                                returnMessage.Write(newPlayer.indexOnServer);
                                returnMessage.Write((byte)newPlayer.team);

                                returnMessage.Write(connectedPlayersActual);

                                for (int i = 0; i < connectedPlayers.Length; i++)
                                {
                                    if(connectedPlayers[i] != null)
                                    {
                                        returnMessage.Write(connectedPlayers[i].indexOnServer);
                                        returnMessage.Write((byte)connectedPlayers[i].team);
                                        returnMessage.Write(connectedPlayers[i].power);
                                    }
                                }

                                server.SendMessage(returnMessage, newPlayer.netConnection, NetDeliveryMethod.ReliableUnordered);
                                
                                if(connectedPlayersActual > 1)
                                {
                                    NetOutgoingMessage newPlayerMessage = server.CreateMessage();
                                    newPlayerMessage.Write((byte)ShapeCustomNetMessageType.NewPlayerJoined);

                                    newPlayerMessage.Write(newPlayer.indexOnServer);
                                    newPlayerMessage.Write((byte)newPlayer.team);
                                    newPlayerMessage.Write(/*newPlayer.power*/5);

                                    server.SendMessage(newPlayerMessage, GetRecipients(newPlayer.indexOnServer), NetDeliveryMethod.ReliableUnordered, 0);
                                }
                                
                                Console.WriteLine("Player connected");
                                break;
                        }
                        break;
                    case NetIncomingMessageType.DiscoveryRequest:
                        NetOutgoingMessage response = server.CreateMessage();
                        response.Write("This is a Shape Space server");
                        server.SendDiscoveryResponse(response, msg.SenderEndPoint);
                        break;
                    case NetIncomingMessageType.ConnectionApproval:
                        Console.WriteLine("A client is asking to connect");
                        //Approve the connection and send back a hailmessage containing server info back
                        NetOutgoingMessage hailMessage = server.CreateMessage();
                        hailMessage.Write(maxPlayers);

                        msg.SenderConnection.Approve(hailMessage);
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        if(msg.ReadByte() == (byte)NetConnectionStatus.Disconnected)
                        {
                            //Console.WriteLine("Player disconnected: " + FindPlayerByNetConnection(msg.SenderConnection).Username);

                            RemovePlayer(msg.SenderConnection);
                        }
                        break;
                    default:
                        Console.WriteLine("Unhandled type: " + msg.MessageType + " Info: \"" + msg.ReadString() + "\"");
                        break;
                }
                server.Recycle(msg);
            }

            //Move players
            UpdatePlayers(deltaSecond);

            //Simulate the world
            physicsWorld.Step(1f/100f);

            //Return data to clients
            if (lastSentData >= 1f/ReturnDataPerSecond && connectedPlayersActual > 0)
            {
                //List<NetConnection> recipients = new List<NetConnection>();

                NetOutgoingMessage outMess = server.CreateMessage();
                outMess.Write((byte)ShapeCustomNetMessageType.LocationUpdate);
                outMess.Write(connectedPlayersActual);

                for (int i = 0; i < maxPlayers; i++)
                {
                    if (connectedPlayers[i] != null)
                    {
                        outMess.Write(connectedPlayers[i].indexOnServer);
                        outMess.Write(connectedPlayers[i].power);
                        outMess.Write(lastSentData);
                        outMess.Write(ConvertUnits.ToDisplayUnits(connectedPlayers[i].body.Position));

                        int trailCount = connectedPlayers[i].trail.Count;
                        outMess.Write(trailCount);
                        
                        for(int j = 0; j < trailCount; j++)
                        {
                            //outMess.Write(connectedPlayers[i].trail[j].Id);
                            outMess.Write(connectedPlayers[i].trail[j].position);
                            outMess.Write(connectedPlayers[i].trail[j].size);
                        }
                    }
                }

                //Write all remnants positions
                int remnantCount = remnants.Count;
                outMess.Write(remnantCount);

                for (int j = 0; j < remnantCount; j++)
                {
                    outMess.Write(remnants[j].position);
                    outMess.Write(remnants[j].size);
                }

                server.SendMessage(outMess, GetRecipients(-1), NetDeliveryMethod.UnreliableSequenced, 2);

                lastSentData = 0;
            }

            //Make sure the server runs at about 60 frames per second
            if(deltaSecond * 1000 < 1000/17)
                Thread.Sleep(Convert.ToInt32(1000/17 - deltaSecond * 1000));

            loopEndTime = Environment.TickCount;
        }
    }

    static void UpdatePlayers(float deltaTime)
    {
        for(int i = 0; i < maxPlayers; i++)
        {
            if (connectedPlayers[i] != null)
                connectedPlayers[i].Update(deltaTime);
        }

        for (int i = 0; i < remnants.Count; i++)
        {
            if (remnants[i] != null)
                remnants[i].Update(deltaTime);
        }
    }

    static void CreateRemnant(Vector2 pos, float size, float angle, int ownerId, Player creator)
    {
        NetworkCollisionRemnant newRemnant = new NetworkCollisionRemnant(pos, size, angle, ownerId, Color.GhostWhite, physicsWorld, creator);
        newRemnant.OnDestroy += RemoveRemnant;
        remnants.Add(newRemnant);
    }

    static void RemoveRemnant(int id)
    {
        for (int i = 0; i < remnants.Count; i++)
        {
            if (remnants[i].Id == id)
            {
                remnants.RemoveAt(i);
                break;
            }
        }
    }

    //Called when a player connects
    static int AddNewPlayer(NetworkPlayer player)
    {
        int spotAssignedInList = FindNextEmptySpot();

        connectedPlayers[spotAssignedInList] = player;

        connectedPlayersActual += 1;

        Console.WriteLine("Added Player");

        return spotAssignedInList;
    }

    //Called when a player disconnects
    static void RemovePlayer(NetConnection connection)
    {
        for (int i = 0; i < maxPlayers; i++ )
        {
            if(connectedPlayers[i] != null)
            {
                if (connectedPlayers[i].netConnection == connection)
                {
                    connectedPlayers[i] = null;
                    connectedPlayersActual -= 1;
                }
            }
        }
    }

    static int FindNextEmptySpot(int startvalue = 0)
    {
        if (connectedPlayersActual == maxPlayers)
            return -1;

        for(int i = startvalue; i < 10; i++)
        {
            if (connectedPlayers[i] == null)
                return i;
        }

        return -1;
    }

    static NetConnection[] GetRecipients(int exclusionIndex)
    {
        NetConnection[] recipients;

        if(exclusionIndex < 0 || exclusionIndex >= maxPlayers)
            recipients = new NetConnection[connectedPlayersActual];
        else
            recipients = new NetConnection[connectedPlayersActual - 1];

        int additions = 0;

        for(int i = 0; i < maxPlayers; i++)
        {
            if (connectedPlayers[i] != null)
                if (connectedPlayers[i].indexOnServer != exclusionIndex)
                {
                    recipients[additions] = connectedPlayers[i].netConnection;
                    additions++;
                }  
        }

        return recipients;
    }

    static NetworkPlayer FindPlayerByNetConnection(NetConnection con)
    {
        for (int i = 0; i < maxPlayers; i++ )
        {
            if(connectedPlayers[i] != null)
            {
                if(connectedPlayers[i].netConnection == con)
                {
                    return connectedPlayers[i];
                }
            }
        }
        
        return null;
    }

    static void WaitForKeyPress()
    {
        Console.Write("Press any key to continue...");
        Console.ReadKey(true);
    }
}
