using System;
using Lidgren.Network;
using ShapeSpaceHelper;

class Program
{
    static void Main(string[] args)
    {
        NetPeerConfiguration config = new NetPeerConfiguration("ShapeSpace");
        config.Port = 55678;
        config.MaximumConnections = 10;
        config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);

        NetServer server = new NetServer(config);
        
        try
        {
            server.Start();
        }
        catch(Exception e)
        {
            Console.WriteLine(e);
            WaitForKeyPress();
        }

        while(true)
        {
            //Message handling loop
            NetIncomingMessage msg;
            while ((msg = server.ReadMessage()) != null)
            {
                Console.WriteLine(msg.MessageType);
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.DiscoveryRequest:
                        NetOutgoingMessage response = server.CreateMessage();
                        response.Write("Name of server");
                        server.SendDiscoveryResponse(response, msg.SenderEndPoint);
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
