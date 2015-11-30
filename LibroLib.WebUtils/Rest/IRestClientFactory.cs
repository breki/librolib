using System.Diagnostics.Contracts;

namespace LibroLib.WebUtils.Rest
{
    [ContractClass(typeof(IRestClientFactoryContract))]
    public interface IRestClientFactory
    {
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
    }
}