using System;
using System.Collections.Generic;
using System.Text;

namespace RelayServer.HolePunching
{
    class HolePunchingClientInfo
    {
        public string IpAddress { get; private set; }
        public int Port { get; private set; }
        public DateTime LastAliveTime { get; private set; }
        public TimeSpan LastAliveElapsed {
                get
            {
                return DateTime.Now - LastAliveTime;
            }
        }

        public HolePunchingClientInfo(string ipAddress, int port)
        {
            IpAddress = ipAddress;
            Port = port;

            LastAliveTime = DateTime.Now;
        }

        public void RenewAliveTime()
        {
            LastAliveTime = DateTime.Now;
        }

        public static bool operator ==(HolePunchingClientInfo reference, HolePunchingClientInfo operand)
        {
            if (reference.IpAddress == operand.IpAddress &&
                reference.Port == operand.Port)
                return true;
            return false;
        }

        public static bool operator !=(HolePunchingClientInfo reference, HolePunchingClientInfo operand)
        {
            if (reference.IpAddress == operand.IpAddress &&
                reference.Port == operand.Port)
                return false;
            return true;
        }
    }
}
