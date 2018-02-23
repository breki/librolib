using System;
using System.Diagnostics.Contracts;
using System.IO;

namespace LibroLib.WebUtils.Ftp
{
    [ContractClass(typeof(IFtpChannelContract))]
    public interface IFtpChannel : IDisposable
    {
        void Connect(byte[] hostAddress, int? port);
        void Connect(string host, int? port);
        void Disconnect();
        void Receive(Stream writeStream);
        int Send(byte[] data);
    }

    [ContractClassFor(typeof(IFtpChannel))]
    internal abstract class IFtpChannelContract : IFtpChannel
    {
        void IDisposable.Dispose()
        {
        }

        void IFtpChannel.Connect(byte[] hostAddress, int? port)
        {
            Contract.Requires(hostAddress != null);
            Contract.Requires(port == null || port.Value <= 0xffff);
        }

        void IFtpChannel.Connect(string host, int? port)
        {
            Contract.Requires(host != null);
            Contract.Requires(port == null || port.Value <= 0xffff);
        }

        void IFtpChannel.Disconnect()
        {
        }

        void IFtpChannel.Receive(Stream writeStream)
        {
            Contract.Requires(writeStream != null);
        }

        int IFtpChannel.Send(byte[] data)
        {
            Contract.Requires(data != null);
            throw new NotImplementedException();
        }
    }
}