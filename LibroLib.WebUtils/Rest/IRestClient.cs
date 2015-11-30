using System;
using System.Diagnostics.Contracts;

namespace LibroLib.WebUtils.Rest
{
    [ContractClass(typeof(IRestClientContract))]
    public interface IRestClient : IDisposable
    {
        IRestClientResponse Response { get; }
        int StatusCode { get; }

        IRestClient AddQuery(string name, object value);
        IRestClient Delete (string url);
        [System.Diagnostics.CodeAnalysis.SuppressMessage ("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Do")]
        IRestClient Do();
        [System.Diagnostics.CodeAnalysis.SuppressMessage ("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Get")]
        IRestClient Get (string url);
        IRestClient Post (string url);
        IRestClient Put (string url);
        IRestClient Request(string text);
        IRestClient WithConfiguration(IWebConfiguration webConfiguration);
        IRestClient WithTimeout(TimeSpan timeout);
    }

    [ContractClassFor(typeof(IRestClient))]
    internal abstract class IRestClientContract : IRestClient
    {
        void IDisposable.Dispose()
        {
        }

        IRestClientResponse IRestClient.Response
        {
            get
            {
                Contract.Ensures(Contract.Result<IRestClientResponse>() != null);
                throw new InvalidOperationException();
            }
        }

        int IRestClient.StatusCode
        {
            get { throw new InvalidOperationException(); }
        }

        IRestClient IRestClient.AddQuery(string name, object value)
        {
            Contract.Requires(name != null);
            Contract.Ensures(Contract.Result<IRestClient>() != null);
            throw new NotImplementedException();
        }

        IRestClient IRestClient.Delete(string url)
        {
            Contract.Requires(url != null);
            Contract.Ensures(Contract.Result<IRestClient>() != null);
            throw new NotImplementedException();
        }

        IRestClient IRestClient.Do()
        {
            Contract.Ensures(Contract.Result<IRestClient>() != null);
            throw new NotImplementedException();
        }

        IRestClient IRestClient.Get(string url)
        {
            Contract.Requires(url != null);
            Contract.Ensures(Contract.Result<IRestClient>() != null);
            throw new NotImplementedException();
        }

        IRestClient IRestClient.Post(string url)
        {
            Contract.Requires(url != null);
            Contract.Ensures(Contract.Result<IRestClient>() != null);
            throw new NotImplementedException();
        }

        IRestClient IRestClient.Put(string url)
        {
            Contract.Requires(url != null);
            Contract.Ensures(Contract.Result<IRestClient>() != null);
            throw new NotImplementedException();
        }

        IRestClient IRestClient.Request(string text)
        {
            Contract.Ensures(Contract.Result<IRestClient>() != null);
            throw new NotImplementedException();
        }

        IRestClient IRestClient.WithConfiguration(IWebConfiguration webConfiguration)
        {
            Contract.Requires(webConfiguration != null);
            Contract.Ensures(Contract.Result<IRestClient>() != null);
            throw new NotImplementedException();
        }

        IRestClient IRestClient.WithTimeout(TimeSpan timeout)
        {
            Contract.Ensures(Contract.Result<IRestClient>() != null);
            throw new NotImplementedException();
        }
    }
}