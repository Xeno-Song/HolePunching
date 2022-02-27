using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UserClient.HolePunching
{
    class HolePunchingUdpClient
    {
        public string TargetIp { get; private set; }
        public int TargetPort { get; private set; }
        public bool IsConnected { get { return socket.Connected; } }

        private Socket socket;

        public HolePunchingUdpClient()
        {
            TargetIp = "";
            TargetPort = 0;
        }

        public HolePunchingUdpClient(string targetIp, int targetPort)
        {
            TargetIp = targetIp;
            TargetPort = targetPort;
        }

        public bool Connect()
        {
            if (TargetIp.Length == 0) return false;
            if (TargetPort == 0) return false;

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            try
            {
                socket.Connect(TargetIp, TargetPort);
            }
            catch (Exception e)
            {
                Console.WriteLine("[HolePunchingUdpClient] Error: Error while socket connecting. " + e.Message);
                Console.WriteLine(e.StackTrace);
                return false;
            }

            if(socket.Connected == false)
            {
                Console.WriteLine("[HolePunchingUdpClient] Error: Faild to connect host");
                return false;
            }

            return true;
        }

        public bool Connect(string targetIp, int targetPort)
        {
            TargetIp = targetIp;
            TargetPort = targetPort;

            return Connect();
        }

        public bool Send(string text)
        {
            if (IsConnected == false) return false;
            int dataSent = socket.Send(Encoding.ASCII.GetBytes(text));

            return text.Length == dataSent;
        }
    }
}
