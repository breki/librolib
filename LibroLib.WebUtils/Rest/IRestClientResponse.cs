using System;
using System.Diagnostics.Contracts;
using Newtonsoft.Json.Linq;

namespace LibroLib.WebUtils.Rest
{
    [ContractClass(typeof(IRestClientResponseContract))]
    public interface IRestClientResponse : IDisposable
    {
        JObject AsJson();
        JArray AsJsonArray();
        string AsString();
    }

    [ContractClassFor(typeof(IRestClientResponse))]
    // ReSharper disable once InconsistentNaming
    internal abstract class IRestClientResponseContract : IRestClientResponse
    {
        void IDisposable.Dispose()
        {
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

        string IRestClientResponse.AsString()
        {
            Contract.Ensures(Contract.Result<string>() != null);
            throw new NotImplementedException();
        }
    }
}