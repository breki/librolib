using System;
using System.Diagnostics.Contracts;
using System.Net;

namespace LibroLib.WebUtils.Rest
{
    [ContractClass(typeof(IRestClientContract))]
    public interface IRestClient : IDisposable
    {
        WebHeaderCollection RequestHeaders { get; }
        IRestClientResponse Response { get; }
        int StatusCode { get; }

        IRestClient AddHeader(string name, string value);
        IRestClient AddHeader (HttpRequestHeader header, string value);
        IRestClient AddQuery (string name, object value);
        IRestClient Delete (string url);
        [System.Diagnostics.CodeAnalysis.SuppressMessage ("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Do")]
        IRestClient Do();
        [System.Diagnostics.CodeAnalysis.SuppressMessage ("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Get")]
        IRestClient Get (string url);
        IRestClient Head (string url);
        IRestClient Post (string url);
        IRestClient Put (string url);
        IRestClient Request(string text);
        IRestClient WithConfiguration(IWebConfiguration webConfiguration);
        IRestClient WithTimeout(TimeSpan timeout);
    }

    [ContractClassFor(typeof(IRestClient))]
    // ReSharper disable once InconsistentNaming
    internal abstract class IRestClientContract : IRestClient
    {
        void IDisposable.Dispose()
        {
        }

        WebHeaderCollection IRestClient.RequestHeaders
        {
            get
            {
                Contract.Ensures(Contract.Result<WebHeaderCollection>() != null);
                throw new NotImplementedException();
            }
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

        IRestClient IRestClient.AddHeader(string name, string value)
        {
            Contract.Requires (name != null);
            Contract.Ensures (ReferenceEquals(this, Contract.Result<IRestClient>()));
            throw new NotImplementedException ();
        }

        IRestClient IRestClient.AddHeader(HttpRequestHeader header, string value)
        {
            Contract.Ensures(ReferenceEquals(this, Contract.Result<IRestClient>()));
            throw new NotImplementedException();
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

        IRestClient IRestClient.Head(string url)
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