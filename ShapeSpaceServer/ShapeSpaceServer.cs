using System;
using Lidgren.Network;
using Lidgren.Network.Xna;
using ShapeSpace.Network;
using FarseerPhysics.Dynamics;

class Program
{
    static void Main(string[] args)
    {
        NetPeerConfiguration config = new NetPeerConfiguration("ShapeSpace");
        config.Port = 55678;
        config.MaximumConnections = 10;
        config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
        config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);

        NetServer server = new NetServer(config);
        
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
                        switch((ShapeCustomNetMessageType)msg.ReadByte())
                        {
                            case ShapeCustomNetMessageType.InputUpdate:
                                int numOfInputs = msg.ReadInt32();
                                for (int i = 0; i < numOfInputs; i++ )
                                {
                                    
                                }
                                    break;
                        }
                        break;
                    case NetIncomingMessageType.DiscoveryRequest:
                        NetOutgoingMessage response = server.CreateMessage();
                        response.Write("Name of server");
                        server.SendDiscoveryResponse(response, msg.SenderEndPoint);
                        break;
                    case NetIncomingMessageType.ConnectionApproval:
                        Console.WriteLine("Recienved connection");
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
}
