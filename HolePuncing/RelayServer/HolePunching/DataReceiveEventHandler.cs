using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RelayServer.HolePunching
{
    class DataReceiveEventHandler : EventArgs
    {
        public HolePunchingClientInfo ClientInfo { get; private set; }
        public List<byte> Data { get; private set; }

        public DataReceiveEventHandler(HolePunchingClientInfo clientInfo, List<byte> data)
        {
            ClientInfo = clientInfo;
            Data = new List<byte>(data);
        }
    }
}
