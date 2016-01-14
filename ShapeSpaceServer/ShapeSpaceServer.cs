using System;
using Lidgren.Network;
using Lidgren.Network.Xna;
using ShapeSpace.Network;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

class Program
{
    static int maxPlayers = 10;

    static NetworkPlayer[] connectedPlayers = new NetworkPlayer[maxPlayers];
    //The number of players that are connected
    static int connectedPlayersActual = 0;

    static void Main(string[] args)
    {
        NetPeerConfiguration config = new NetPeerConfiguration("ShapeSpace");
        config.Port = 55678;
        config.MaximumConnections = maxPlayers;
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

        while(true)
        {
            //Message handling loop
            NetIncomingMessage msg;
            while ((msg = server.ReadMessage()) != null)
            {
                Console.WriteLine(msg.MessageType);
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        //A swith handling ShapeSpace custom message types
                        switch((ShapeCustomNetMessageType)msg.ReadByte())
                        {
                            case ShapeCustomNetMessageType.InputUpdate:
                                int numOfInputs = msg.ReadInt32();
                                for (int i = 0; i < numOfInputs; i++ )
                                {
                                    
                                }
                                break;
                            case ShapeCustomNetMessageType.SetupRequest:
                                NetworkPlayer newPlayer = new NetworkPlayer(physicsWorld, msg.SenderConnection);
                                
                                NetOutgoingMessage returnMessage = server.CreateMessage();

                                try
                                {
                                    newPlayer.SetTeam((ShapeTeam)msg.ReadByte());
                                }
                                catch(Exception e)
                                {
                                    returnMessage.Write((byte)ShapeCustomNetMessageType.SetupFailed);
                                    returnMessage.Write(e.Message);
                                    server.SendMessage(returnMessage, newPlayer.netConnection, NetDeliveryMethod.ReliableUnordered);
                                    break;
                                }
                                returnMessage.Write((byte)ShapeCustomNetMessageType.SetupSuccessful);
                                server.SendMessage(returnMessage, newPlayer.netConnection, NetDeliveryMethod.ReliableUnordered);
                                break;
                        }
                        break;
                    case NetIncomingMessageType.DiscoveryRequest:
                        NetOutgoingMessage response = server.CreateMessage();
                        response.Write("This is a Shape Space server");
                        server.SendDiscoveryResponse(response, msg.SenderEndPoint);
                        break;
                    case NetIncomingMessageType.ConnectionApproval:
                        Console.WriteLine("Recieved connection");
                        //Approve the connection and send back a hailmessage containing server info back
                        msg.SenderConnection.Approve();
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        break;
                    default:
                        Console.WriteLine("Unhandled type: " + msg.MessageType + " Info: \"" + msg.ReadString() + "\"");
                        break;
                }
                server.Recycle(msg);
            }
        }
    }

    static void WaitForKeyPress()
    {
        Console.Write("Press any key to continue...");
        Console.ReadKey(true);
    }

    //Called when a player connects
    static void AddNewPlayer(NetworkPlayer player)
    {
        connectedPlayers[FindNextEmptySpot()] = player;

        connectedPlayersActual += 1;
    }

    //Called when a player disconnects
    static void RemovePlayer(NetworkPlayer player)
    {
        for (int i = 0; i < maxPlayers; i++ )
        {
            if (connectedPlayers[i] == player)
            {
                connectedPlayers[i] = null;
                connectedPlayersActual -= 1;
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

    static void SendMessageToPlayer(NetworkPlayer player)
    {
        
    }
}
