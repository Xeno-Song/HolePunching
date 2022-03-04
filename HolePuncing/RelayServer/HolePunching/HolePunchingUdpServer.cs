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
        public int ClientTimeout { get; set; }
        public event EventHandler<DataReceiveEventHandler> OnDataRecv;
        public event EventHandler<HolePunchingClientInfo> OnTimeout;

        private Socket serverSocket;
        private Thread recvThread;
        private List<HolePunchingClientInfo> clientInfoList;

        public HolePunchingUdpServer()
        {
            serverSocket = null;
            recvThread = null;
            clientInfoList = new List<HolePunchingClientInfo>();
            ClientTimeout = 10000;
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

                    HolePunchingClientInfo clientInfo = new HolePunchingClientInfo(
                        remoteEndPoint.Address.ToString(), remoteEndPoint.Port);

                    bool foundClientInfo = false;
                    foreach (var clientInfoIter in clientInfoList)
                    {
                        if (clientInfo == clientInfoIter)
                        {
                            clientInfoIter.RenewAliveTime();
                            foundClientInfo = true;
                            break;
                        }
                    }

                    if (foundClientInfo == false)
                        clientInfoList.Add(clientInfo);

                    DataReceiveEventHandler eventArgs = new DataReceiveEventHandler(
                        clientInfo, recvData);
                    OnDataRecv?.Invoke(this, eventArgs);

                    asyncResult = null;
                }

                for (int i = 0; i < clientInfoList.Count; i++)
                {
                    if (clientInfoList[i].LastAliveElapsed.TotalMilliseconds >= ClientTimeout)
                    {
                        OnTimeout?.Invoke(this, clientInfoList[i]);
                        clientInfoList.RemoveAt(i);
                        --i;
                    }
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
