using RelayServer.HolePunching;
using System;

namespace RelayServer
{
    class Program
    {
        static void Main(string[] args)
        {
            HolePunchingServer server = new HolePunchingServer();
            server.Main();
        }
    }
}
