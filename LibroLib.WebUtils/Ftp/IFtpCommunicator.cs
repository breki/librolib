using System.Diagnostics.Contracts;

namespace LibroLib.WebUtils.Ftp
{
    [ContractClass(typeof(IFtpCommunicatorContract))]
    public interface IFtpCommunicator
    {
        void AttachToChannel(IFtpChannel channel);
        void DetachFromChannel();
        FtpServerResponse ReadResponse();
        void SendCommand(string command);
    }

    [ContractClassFor(typeof(IFtpCommunicator))]
    internal abstract class IFtpCommunicatorContract : IFtpCommunicator
    {
        void IFtpCommunicator.AttachToChannel(IFtpChannel channel)
        {
            Contract.Requires(channel != null);
        }

        void IFtpCommunicator.DetachFromChannel()
        {
        }

        FtpServerResponse IFtpCommunicator.ReadResponse()
        {
            Contract.Ensures(Contract.Result<FtpServerResponse>() != null);
            throw new System.NotImplementedException();
        }

        void IFtpCommunicator.SendCommand(string command)
        {
            Contract.Requires(command != null);
        }
    }
}