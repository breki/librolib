using System.Diagnostics.Contracts;
using LibroLib.Misc;

namespace LibroLib.WebUtils.Ftp
{
    [ContractClass(typeof(IFtpChannelFactoryContract))]
    public interface IFtpChannelFactory : IFactory
    {
        IFtpChannel CreateChannel();
    }

    [ContractClassFor(typeof(IFtpChannelFactory))]
    internal abstract class IFtpChannelFactoryContract : IFtpChannelFactory
    {
        void IFactory.Destroy(object component)
        {
        }

        IFtpChannel IFtpChannelFactory.CreateChannel()
        {
            Contract.Ensures(Contract.Result<IFtpChannel>() != null);
            throw new System.NotImplementedException();
        }
    }
}