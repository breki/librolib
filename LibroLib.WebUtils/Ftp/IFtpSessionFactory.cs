using System.Diagnostics.Contracts;
using LibroLib.Misc;

namespace LibroLib.WebUtils.Ftp
{
    [ContractClass(typeof(IFtpSessionFactoryContract))]
    public interface IFtpSessionFactory : IFactory
    {
        IFtpSession CreateSession();
    }

    [ContractClassFor(typeof(IFtpSessionFactory))]
    internal abstract class IFtpSessionFactoryContract : IFtpSessionFactory
    {
        void IFactory.Destroy(object component)
        {
        }

        IFtpSession IFtpSessionFactory.CreateSession()
        {
            Contract.Ensures(Contract.Result<IFtpSession>() != null);
            throw new System.NotImplementedException();
        }
    }
}