using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UserClient.HolePunching
{
    class HolePunchingUdpSocket : IDisposable
    {
        public string TargetIp { get; private set; }
        public int TargetPort { get; private set; }
        public bool IsConnected { get { return socket.Connected; } }
        public int RecvBufferSize {
            get
            {
                if (recvBuffer == null) return 0;
                return recvBuffer.Length;
            }
            set
            {
                recvBuffer = null;
                recvBuffer = new byte[value];
            }
        }

        private Socket socket;
        private Thread recvThread;
        private byte[] recvBuffer;

        public HolePunchingUdpSocket()
        {
            TargetIp = "";
            TargetPort = 0;
        }

        public HolePunchingUdpSocket(string targetIp, int targetPort)
        {
            TargetIp = targetIp;
            TargetPort = targetPort;
        }

        public bool Connect()
        {
            if (TargetIp.Length == 0) return false;
            if (TargetPort == 0) return false;

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.ReceiveTimeout = 100;
            socket.SendTimeout = 100;

            if (RecvBufferSize == 0) RecvBufferSize = 512;

            try
            {
                socket.Connect(TargetIp, TargetPort);
            }
            catch (Exception e)
            {
                Debug.WriteLine("[HolePunchingUdpClient] Error: Error while socket connecting. " + e.Message);
                Debug.WriteLine(e.StackTrace);
                return false;
            }

            if (socket.Connected == false)
            {
                Debug.WriteLine("[HolePunchingUdpClient] Error: Faild to connect host");
                return false;
            }

            if (recvThread != null && recvThread.IsAlive)
            {
                recvThread.Abort();
                recvThread = null;
            }

            Debug.WriteLine(socket.LocalEndPoint.ToString());

            recvThread = new Thread(RecvThreadJob);
            recvThread.Start();

            return true;
        }

        public bool Connect(string targetIp, int targetPort)
        {
            TargetIp = targetIp;
            TargetPort = targetPort;

            return Connect();
        }

        public void Disconnect()
        {
            if (socket != null)
            {
                socket.Close();
                
                if (recvThread != null &&
                    recvThread.IsAlive)
                {
                    recvThread.Join();
                }

                socket.Dispose();
            }
        }

        public bool Send(string text)
        {
            if (IsConnected == false) return false;
            int dataSent = socket.Send(Encoding.ASCII.GetBytes(text));

            return text.Length == dataSent;
        }

        public bool SendGroupNum(int num)
        {
            if (IsConnected == false) return false;
            byte[] data = BitConverter.GetBytes(num);
            int dataSent = socket.Send(data);

            return (data.Length == dataSent);
        }

        private void RecvThreadJob()
        {
            while (IsConnected)
            {
                int recvSize = Recv(out List<byte> dataArray);
                if(recvSize != 0)
                    Debug.WriteLine(Encoding.ASCII.GetString(dataArray.ToArray()));
                else Thread.Sleep(1);
            }
        }

        private int Recv(out List<byte> dataArray)
        {
            dataArray = null;
            if (socket.Available == 0) return 0;

            int recvSize = socket.Receive(recvBuffer, RecvBufferSize, SocketFlags.None);
            dataArray = new List<byte>(recvBuffer);
            dataArray.RemoveRange(recvSize, dataArray.Count - recvSize);

            return recvSize;
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
