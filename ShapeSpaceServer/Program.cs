using System;
using Lidgren.Network;

class Program
{
    static void Main(string[] args)
    {
        NetPeerConfiguration config = new NetPeerConfiguration("ShapeSpace");
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
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.DiscoveryRequest:
                        break;
                    default:
                        Console.WriteLine("Unhandled type: " + msg.MessageType);
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
