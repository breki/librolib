using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LibroLib.WebUtils.Rest
{
    public class RestClientResponse : IRestClientResponse
    {
        public RestClientResponse([NotNull] WebRequest webRequest, bool requestBodySet)
        {
            Contract.Requires(webRequest != null);

            //log.DebugFormat("Sending REST request {1} '{0}'", webRequest.RequestUri, webRequest.Method);

            if (!requestBodySet)
                webRequest.ContentLength = 0;

            try
            {
                webResponse = (HttpWebResponse)webRequest.GetResponse();
            }
            catch (WebException ex)
            {
                throw new RestException("Exception on REST {0} call to {1}".Fmt(webRequest.Method, webRequest.RequestUri), ex);
            }

            responseStream = webResponse.GetResponseStream();
        }

        public int StatusCode
        {
            get { return (int)webResponse.StatusCode; }
        }

        public WebHeaderCollection Headers
        {
            get { return webResponse.Headers; }
        }

        public byte[] AsBytes()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                responseStream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public JObject AsJson()
        {
            JsonSerializer serializer = new JsonSerializer();

            using (var sr = new StreamReader(responseStream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return JObject.FromObject(serializer.Deserialize(jsonTextReader), serializer);
            }
        }

        public JArray AsJsonArray()
        {
            JsonSerializer serializer = new JsonSerializer();

            using (var sr = new StreamReader(responseStream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return JArray.FromObject(serializer.Deserialize(jsonTextReader), serializer);
            }
        }

        public Stream AsStream()
        {
            return responseStream;
        }

        public string AsString()
        {
            using (StreamReader reader = new StreamReader(responseStream))
                return reader.ReadToEnd();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
                return;
            // clean native resources

            if (disposing)
            {
                // clean managed resources
                if (webResponse != null)
                {
                    webResponse.Close();
                    webResponse = null;
                }
            }

            disposed = true;
        }

        private HttpWebResponse webResponse;
        private bool disposed;
        private readonly Stream responseStream;
        //private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}