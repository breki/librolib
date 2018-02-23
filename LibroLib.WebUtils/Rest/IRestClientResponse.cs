using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;

namespace LibroLib.WebUtils.Rest
{
    [ContractClass(typeof(IRestClientResponseContract))]
    public interface IRestClientResponse : IDisposable
    {
        [NotNull]
        WebHeaderCollection Headers { get; }

        [NotNull]
        byte[] AsBytes();
        [NotNull]
        JObject AsJson();
        [NotNull]
        JArray AsJsonArray();
        [NotNull]
        Stream AsStream();
        [NotNull]
        string AsString();
    }

    [ContractClassFor(typeof(IRestClientResponse))]
    // ReSharper disable once InconsistentNaming
    internal abstract class IRestClientResponseContract : IRestClientResponse
    {
        WebHeaderCollection IRestClientResponse.Headers
        {
            get
            {
                Contract.Ensures(Contract.Result<WebHeaderCollection>() != null);
                throw new NotImplementedException();
            }
        }

        void IDisposable.Dispose()
        {
        }

        byte[] IRestClientResponse.AsBytes()
        {
            Contract.Ensures(Contract.Result<byte[]>() != null);
            throw new NotImplementedException();
        }

        JObject IRestClientResponse.AsJson()
        {
            Contract.Ensures(Contract.Result<JObject>() != null);
            throw new NotImplementedException();
        }

        JArray IRestClientResponse.AsJsonArray()
        {
            Contract.Ensures(Contract.Result<JArray>() != null);
            throw new NotImplementedException();
        }

        Stream IRestClientResponse.AsStream()
        {
            Contract.Ensures(Contract.Result<Stream>() != null);
            throw new NotImplementedException();
        }

        string IRestClientResponse.AsString()
        {
            Contract.Ensures(Contract.Result<string>() != null);
            throw new NotImplementedException();
        }
    }
}