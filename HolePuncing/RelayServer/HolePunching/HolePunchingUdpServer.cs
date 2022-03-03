using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace RelayServer.HolePunching
{
    class HolePunchingUdpServer : IDisposable
    {
        public short BindPort { get; private set; }
        public bool IsBound
        {
            get
            {
                if (serverSocket == null) return false;
                else return serverSocket.IsBound;
            }
        }
        public bool Disposed { get; private set; }
        public event EventHandler<DataReceiveEventHandler> OnDataRecv;

        private Socket serverSocket;
        private Thread recvThread;
           
        public HolePunchingUdpServer()
        {
            serverSocket = null;
            recvThread = null;
        }

        public bool Bind(short port)
        {
            if (IsBound == true)
                return false;

            BindPort = port;
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, BindPort);

            serverSocket = new Socket(SocketType.Dgram, ProtocolType.Udp);
            serverSocket.Bind(endPoint);

            recvThread = new Thread(SocketRecvThreadJob);
            recvThread.Start();

            return true;
        }

        private void SocketRecvThreadJob()
        {
            IAsyncResult asyncResult = null;
            byte[] buffer = new byte[2048];
            EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);

            while (true)
            {
                if (Disposed == true) return;
                if (serverSocket == null) return;
                if (serverSocket.IsBound == false) return;

                if (asyncResult == null)
                    asyncResult = serverSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref endPoint, null, null);

                if (asyncResult.AsyncWaitHandle.WaitOne(100))
                {
                    // When socket accpeted
                    int dataSize = serverSocket.EndReceiveFrom(asyncResult, ref endPoint);

                    IPEndPoint remoteEndPoint = endPoint as IPEndPoint;
                    List<byte> recvData = new List<byte>(buffer);
                    recvData.RemoveRange(dataSize, recvData.Count - dataSize);
                    
                    DataReceiveEventHandler eventArgs = new DataReceiveEventHandler(
                        remoteEndPoint.Address.ToString(), remoteEndPoint.Port, recvData);
                    OnDataRecv?.Invoke(this, eventArgs);

                    asyncResult = null;
                }
            }
        }

        public void Dispose()
        {
            Disposed = true;

            if (recvThread != null) recvThread.Join();
            if(serverSocket != null)
            {
                serverSocket.Close();
                serverSocket.Dispose();
                serverSocket = null;
            }
        }
    }
}
