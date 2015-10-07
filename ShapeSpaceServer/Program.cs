﻿using Lidgren.Network;
using System;
using System.Net;

class Program
{
    static void Main(string[] args)
    {
        if(args.Length > 0)
        {
            Console.WriteLine("Version: " + args[0]);
        }

        Console.WriteLine("Creating configuration...");

        NetPeerConfiguration config = new NetPeerConfiguration("ShapeSpace");
        config.MaximumConnections = 10;
        config.Port = 5566;
        config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);

        Console.WriteLine("Starting server...");
        NetServer server = new NetServer(config);
        server.Start();
        Console.WriteLine("Server started successfully!");
        
        bool serverRunning = true;
        while(serverRunning)
        {
            NetIncomingMessage message = server.ReadMessage();
            if (message != null)
            {
                switch (message.MessageType)
                {
                    case NetIncomingMessageType.ConnectionApproval:
                        Console.WriteLine(message.ReadString());
                        message.SenderConnection.Approve();
                        break;
                }
            }
            if (Console.KeyAvailable)
                if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                    serverRunning = false;
        }

        server.Shutdown("Server shutting down!");
    }
}