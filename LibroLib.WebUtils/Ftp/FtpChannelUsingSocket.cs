using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace LibroLib.WebUtils.Ftp
{
    public class FtpChannelUsingSocket : IFtpChannel
    {
        public void Connect(byte[] hostAddress, int? port)
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endPoint = new IPEndPoint(new IPAddress(hostAddress), port ?? 21);
            clientSocket.Connect(endPoint);
        }

        public void Connect(string host, int? port)
        {
            Connect(Dns.GetHostEntry(host).AddressList[0].GetAddressBytes(), port);
        }

        public void Disconnect()
        {
            if (clientSocket != null)
            {
                clientSocket.Close();
                clientSocket = null;
            }
        }

        public void Receive(Stream writeStream)
        {
            byte[] buffer = new byte[1000];

            while (true)
            {
                int bytesRead = clientSocket.Receive(buffer);
                writeStream.Write(buffer, 0, bytesRead);
                if (bytesRead < buffer.Length)
                    break;
            }
        }

        public int Send(byte[] data)
        {
            return clientSocket.Send(data);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (false == disposed)
            {
                if (disposing)
                {
                    Disconnect();
                }

                disposed = true;
            }
        }

        private Socket clientSocket;
        private bool disposed;
    }
}