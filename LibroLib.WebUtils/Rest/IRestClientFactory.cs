using System.Diagnostics.Contracts;
using JetBrains.Annotations;
using LibroLib.Misc;

namespace LibroLib.WebUtils.Rest
{
    [ContractClass(typeof(IRestClientFactoryContract))]
    public interface IRestClientFactory : IFactory
    {
        [NotNull]
        IRestClient CreateRestClient();
    }

    [ContractClassFor(typeof(IRestClientFactory))]
    // ReSharper disable once InconsistentNaming
    internal abstract class IRestClientFactoryContract : IRestClientFactory
    {
        IRestClient IRestClientFactory.CreateRestClient()
        {
            Contract.Ensures(Contract.Result<IRestClient>() != null);
            throw new System.NotImplementedException();
        }

        void IFactory.Destroy(object component)
        {
            throw new System.NotImplementedException();
        }
    }
}