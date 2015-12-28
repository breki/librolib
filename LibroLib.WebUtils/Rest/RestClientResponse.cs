using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LibroLib.WebUtils.Rest
{
    public class RestClientResponse : IRestClientResponse
    {
        public RestClientResponse(WebRequest webRequest, bool requestBodySet)
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
#if NET35
            const int BufferSize = 1024 * 1024;
            byte[] buffer = new byte[BufferSize];

            using (MemoryStream memStream = new MemoryStream())
            using (BinaryReader reader = new BinaryReader(responseStream))
            {
                int index = 0;
                
                while (true)
                {
                    int actuallyRead = reader.Read(buffer, index, BufferSize);
                    memStream.Write(buffer, index, actuallyRead);
                    index += actuallyRead;

                    if (actuallyRead < BufferSize)
                    {
                        memStream.Flush();
                        return memStream.ToArray();
                    }
                }
            }
#else
            using (MemoryStream memoryStream = new MemoryStream ())
            {
                responseStream.CopyTo (memoryStream);
                return memoryStream.ToArray ();
            }
#endif
        }

        public JObject AsJson()
        {
            JsonSerializer serializer = new JsonSerializer ();

            using (var sr = new StreamReader (responseStream))
            using (var jsonTextReader = new JsonTextReader (sr))
            {
                return JObject.FromObject(serializer.Deserialize (jsonTextReader), serializer);
            }
        }

        public JArray AsJsonArray()
        {
            JsonSerializer serializer = new JsonSerializer ();

            using (var sr = new StreamReader (responseStream))
            using (var jsonTextReader = new JsonTextReader (sr))
            {
                return JArray.FromObject(serializer.Deserialize (jsonTextReader), serializer);
            }
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

        protected virtual void Dispose(bool disposing)
        {
            if (false == disposed)
            {
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
        }

        private HttpWebResponse webResponse;
        private bool disposed;
        private readonly Stream responseStream;
        //private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}