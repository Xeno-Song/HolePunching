using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RelayServer.HolePunching
{
    class DataReceiveEventHandler : EventArgs
    {
        public string IpAddress { get; private set; }
        public int Port { get; private set; }
        public List<byte> Data { get; private set; }

        public DataReceiveEventHandler(string remoteIp, int remotePort, List<byte> data)
        {
            IpAddress = remoteIp;
            Port = remotePort;
            Data = new List<byte>(data);
        }
    }
}
