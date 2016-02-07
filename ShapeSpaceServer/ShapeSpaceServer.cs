﻿using System;
using Lidgren.Network;
using Lidgren.Network.Xna;
using ShapeSpace.Network;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Factories;
using System.Threading;

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
        const float ReturnDataPerSecond = 20;
        float lastSentData = 0;

        NetPeerConfiguration config = new NetPeerConfiguration("ShapeSpace");
        config.Port = 55678;
        config.MaximumConnections = maxPlayers;
        config.ConnectionTimeout = 10;
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
                                float timeSinceLast = msg.ReadFloat();
                                Vector2 input = msg.ReadVector2();

                                connectedPlayers[FindPlayerByNetConnection(msg.SenderConnection).PlayerIndex].inputs.Add(new InputWithTime(timeSinceLast,input));

                                //Console.WriteLine(timeSinceLast + ": " + input.ToString());
                                break;
                            case ShapeCustomNetMessageType.SetupRequest:
                                NetOutgoingMessage returnMessage = server.CreateMessage();

                                NetworkPlayer newPlayer = new NetworkPlayer(ref physicsWorld, msg.SenderConnection, new Vector2(100,100));
                                
                                try
                                {
                                    newPlayer.SetTeam((ShapeTeam)msg.ReadByte());
                                    newPlayer.SetUserName(msg.ReadString());

                                    int spot = AddNewPlayer(newPlayer);

                                    newPlayer.PlayerIndex = spot;
                                }
                                catch(Exception e)
                                {
                                    returnMessage.Write((byte)ShapeCustomNetMessageType.SetupFailed);
                                    returnMessage.Write(e.Message);
                                    server.SendMessage(returnMessage, newPlayer.netConnection, NetDeliveryMethod.ReliableOrdered);
                                    break;
                                }
                                returnMessage.Write((byte)ShapeCustomNetMessageType.SetupSuccessful);
                                server.SendMessage(returnMessage, newPlayer.netConnection, NetDeliveryMethod.ReliableUnordered);

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
                        msg.SenderConnection.Approve();
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
            physicsWorld.Step(1f/60f);

            //Return data to clients
            if (lastSentData >= 1f/ReturnDataPerSecond)
            {
                for (int i = 0; i < maxPlayers; i++)
                {
                    if (connectedPlayers[i] != null)
                    {
                        NetOutgoingMessage outMess = server.CreateMessage();

                        outMess.Write((byte)ShapeCustomNetMessageType.LocationUpdate);
                        outMess.Write(lastSentData);
                        outMess.Write(connectedPlayers[i].body.Position);

                        server.SendMessage(outMess, connectedPlayers[i].netConnection, NetDeliveryMethod.Unreliable);

                        lastSentData = 0;
                    }
                }
            }

            //Make sure the server runs at about 60 frames per second
            Thread.Sleep(1000/60);

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
