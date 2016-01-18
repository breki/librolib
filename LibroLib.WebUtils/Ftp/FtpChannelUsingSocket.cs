using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
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
            
            int portToUse = port ?? 21;
            Contract.Assume(portToUse >= 0 && portToUse <= 0xffff);
            IPEndPoint endPoint = new IPEndPoint(new IPAddress(hostAddress), portToUse);
            
            clientSocket.Connect(endPoint);
        }

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
        public void Connect(string host, int? port)
        {
            IPHostEntry ipHostEntry = Dns.GetHostEntry(host);
            Contract.Assume (ipHostEntry.AddressList.Length > 0);
            IPAddress ipAddress = ipHostEntry.AddressList[0];
            Contract.Assume (ipAddress != null);
            
            Connect(ipAddress.GetAddressBytes(), port);
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

                Contract.Assume(bytesRead <= buffer.Length);

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