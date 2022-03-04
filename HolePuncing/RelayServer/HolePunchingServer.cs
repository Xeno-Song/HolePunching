using RelayServer.HolePunching;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace RelayServer
{
    class HolePunchingServer
    {
        public HolePunchingServer()
        {

        }

        public void Main()
        {
            HolePunchingUdpServer server = new HolePunchingUdpServer();
            server.OnDataRecv += ClientConnectionEventHandler;
            server.OnTimeout += ClientConnectionTimeout;
            server.Bind(4312);

            while (true)
            {
                string keyInput = Console.ReadLine();
                if (keyInput.StartsWith('q')) break;
            }

            server.Dispose();
        }

        void ClientConnectionEventHandler(object sender, DataReceiveEventHandler eventArgs)
        {
            Console.WriteLine("Client data received : " + eventArgs.ClientInfo.IpAddress + ":" + eventArgs.ClientInfo.Port);
            Console.WriteLine(Encoding.ASCII.GetString(eventArgs.Data.ToArray()));
        }

        void ClientConnectionTimeout(object sender, HolePunchingClientInfo clientInfo)
        {
            Console.WriteLine("Client Timeout : " + clientInfo.IpAddress + ":" + clientInfo.Port);
        }
    }
}
