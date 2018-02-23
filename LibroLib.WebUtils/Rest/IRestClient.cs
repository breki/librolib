using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Net;
using JetBrains.Annotations;

namespace LibroLib.WebUtils.Rest
{
    [ContractClass(typeof(IRestClientContract))]
    public interface IRestClient : IDisposable
    {
        [NotNull]
        WebHeaderCollection RequestHeaders { get; }
        IRestClientResponse Response { get; }
        int StatusCode { get; }

        /// <summary>
        /// Adds the specified cookie to the request.
        /// </summary>
        /// <param name="cookie">The cookie to add.</param>
        /// <returns>This same instance of <see cref="IRestClient"/>.</returns>
        [NotNull]
        IRestClient AddCookie([NotNull] Cookie cookie);
        [NotNull]
        IRestClient AddHeader([NotNull] string name, [CanBeNull] string value);
        [NotNull]
        IRestClient AddHeader(HttpRequestHeader header, [CanBeNull] string value);
        [NotNull]
        IRestClient AddQuery([NotNull] string name, [CanBeNull] object value);
        [NotNull]
        IRestClient Credentials([CanBeNull] ICredentials credentials);
        [NotNull]
#pragma warning disable CA1054 // Uri parameters should not be strings
        IRestClient Delete([NotNull] string url);
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = nameof(Do))]
        [NotNull]
        IRestClient Do();
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = nameof(Get))]
        [NotNull]
        IRestClient Get([NotNull] string url);
        [NotNull]
        IRestClient Head([NotNull] string url);
        [NotNull]
        IRestClient Post([NotNull] string url);
        [NotNull]
        IRestClient PreAuthenticate();
        [NotNull]
        IRestClient Put([NotNull] string url);
#pragma warning restore CA1054 // Uri parameters should not be strings

        /// <summary>
        /// Specifies the request body as a string text.
        /// </summary>
        /// <param name="text">The text of the request.</param>
        /// <returns>This same instance of <see cref="IRestClient"/>.</returns>
        [NotNull]
        IRestClient Request([NotNull] string text);
        [NotNull]
        IRestClient UseDefaultCredentials();
        [NotNull]
        IRestClient WithConfiguration([NotNull] IWebConfiguration webConfiguration);
        [NotNull]
        IRestClient WithTimeout(TimeSpan timeout);
    }

    [ContractClassFor(typeof(IRestClient))]
    // ReSharper disable once InconsistentNaming
    internal abstract class IRestClientContract : IRestClient
    {
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

        void IDisposable.Dispose()
        {
        }

        public IRestClient AddCookie(Cookie cookie)
        {
            Contract.Requires(cookie != null);
            Contract.Ensures(ReferenceEquals(this, Contract.Result<IRestClient>()));
            throw new NotImplementedException();
        }

        IRestClient IRestClient.AddHeader(string name, string value)
        {
            Contract.Requires(name != null);
            Contract.Ensures(ReferenceEquals(this, Contract.Result<IRestClient>()));
            throw new NotImplementedException();
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

        IRestClient IRestClient.Credentials(ICredentials credentials)
        {
            Contract.Requires(credentials != null);
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

        IRestClient IRestClient.PreAuthenticate()
        {
            Contract.Ensures(Contract.Result<IRestClient>() != null);
            throw new NotImplementedException();
        }

        IRestClient IRestClient.Put(string url)
        {
            Contract.Requires(url != null);
            Contract.Ensures(Contract.Result<IRestClient>() != null);
            throw new NotImplementedException();
        }

        IRestClient IRestClient.UseDefaultCredentials()
        {
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