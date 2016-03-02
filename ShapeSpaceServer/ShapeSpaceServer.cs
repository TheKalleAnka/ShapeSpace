using System;
using Lidgren.Network;
using Lidgren.Network.Xna;
using ShapeSpace.Network;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Factories;
using System.Threading;
using System.Collections.Generic;

class Program
{
    static int maxPlayers = 10;

    static NetworkPlayer[] connectedPlayers = new NetworkPlayer[maxPlayers];
    //The number of players that are connected
    static int connectedPlayersActual = 0;

    static void Main(string[] args)
    {
        int loopStartTime = 0;
        int loopEndTime = 0;
        float deltaSecond = 0;

        //Frequency to return data
        const float ReturnDataPerSecond = 80;
        float lastSentData = 0;

        NetPeerConfiguration config = new NetPeerConfiguration("ShapeSpace");
        config.Port = 55678;
        config.MaximumConnections = maxPlayers;
        config.ConnectionTimeout = 5;
        config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
        config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);

        NetServer server = new NetServer(config);

        World physicsWorld = new World(Vector2.Zero);

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

                                NetworkPlayer newPlayer = new NetworkPlayer(ref physicsWorld, msg.SenderConnection, new Vector2(0,0));

                                ShapeTeam team = (ShapeTeam)msg.ReadByte();
                                string username = msg.ReadString();

                                try
                                {
                                    newPlayer.SetTeam(team);
                                    newPlayer.SetUserName(username);

                                    int spot = AddNewPlayer(newPlayer);

                                    newPlayer.indexOnServer = spot;
                                }
                                catch(Exception e)
                                {
                                    returnMessage.Write((byte)ShapeCustomNetMessageType.SetupFailed);
                                    returnMessage.Write(e.Message);
                                    server.SendMessage(returnMessage, newPlayer.netConnection, NetDeliveryMethod.ReliableOrdered);
                                    break;
                                }
                                
                                returnMessage.Write((byte)ShapeCustomNetMessageType.SetupSuccessful);
                                returnMessage.Write(newPlayer.indexOnServer);

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
                                    newPlayerMessage.Write(newPlayer.power);

                                    server.SendMessage(newPlayerMessage, GetRecipientsWithExclusion(newPlayer.indexOnServer), NetDeliveryMethod.ReliableUnordered, 0);
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
                            Console.WriteLine("Player disconnected: " + FindPlayerByNetConnection(msg.SenderConnection).Username);

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
                List<NetConnection> recipients = new List<NetConnection>();

                NetOutgoingMessage outMess = server.CreateMessage();
                outMess.Write((byte)ShapeCustomNetMessageType.LocationUpdate);
                outMess.Write(connectedPlayersActual);

                for (int i = 0; i < maxPlayers; i++)
                {
                    if (connectedPlayers[i] != null)
                    {
                        outMess.Write(i);

                        outMess.Write(lastSentData);
                        outMess.Write(connectedPlayers[i].body.Position);

                        recipients.Add(connectedPlayers[i].netConnection);
                    }
                }

                server.SendMessage(outMess, recipients, NetDeliveryMethod.UnreliableSequenced, 0);

                lastSentData = 0;
            }

            //Make sure the server runs at about 10 frames per second
            Thread.Sleep(1000/100);

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

    static NetConnection[] GetRecipientsWithExclusion(int exclusionIndex)
    {
        NetConnection[] recipients = new NetConnection[connectedPlayersActual - 1];

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
