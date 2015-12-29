using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;

namespace LibroLib.WebUtils.Rest
{
    public class RestClient : IRestClient
    {
        public IRestClientResponse Response
        {
            get { return response; }
        }

        public int StatusCode
        {
            get { return response.StatusCode; }
        }

        public HttpWebRequest NativeWebRequest
        {
            get { return webRequest; }
        }

        public IRestClient AddQuery(string name, object value)
        {
            queryParameters.Add(name, Convert.ToString(value, CultureInfo.InvariantCulture));
            return this;
        }

        public IRestClient Delete(string url)
        {
            this.url = url;
            method = "DELETE";
            return this;
        }

        public IRestClient Get(string url)
        {
            this.url = url;
            method = "GET";
            return this;
        }

        public IRestClient Head(string url)
        {
            this.url = url;
            method = "HEAD";
            return this;
        }

        public IRestClient Post(string url)
        {
            this.url = url;
            method = "POST";
            return this;
        }

        public IRestClient Put(string url)
        {
            this.url = url;
            method = "PUT";
            return this;
        }

        public IRestClient Request(string text)
        {
            requestString = text;
            requestBodySet = true;
            return this;
        }

        public IRestClient WithConfiguration(IWebConfiguration webConfiguration)
        {
            this.webConfiguration = webConfiguration;
            return this;
        }

        public IRestClient WithTimeout(TimeSpan timeout)
        {
            this.timeout = timeout;
            return this;
        }

        public IRestClient Do()
        {
            PrepareWebRequest ();

            if (timeout != null)
                webRequest.Timeout = (int)timeout.Value.TotalMilliseconds;
            if (webConfiguration != null)
                webRequest.UserAgent = webConfiguration.UserAgent;

            if (requestBodySet)
            {
                using (Stream requestStream = webRequest.GetRequestStream ())
                using (StreamWriter writer = new StreamWriter (requestStream))
                    writer.Write (requestString);
            }

            response = new RestClientResponse(webRequest, requestBodySet);
            return this;
        }

        public void Dispose ()
        {
            Dispose (true);
            GC.SuppressFinalize (this);
        }

        protected virtual void Dispose (bool disposing)
        {
            if (false == disposed)
            {
                // clean native resources         

                if (disposing)
                {
                    if (response != null)
                    {
                        response.Dispose();
                        response = null;
                    }
                }

                disposed = true;
            }
        }

        private void PrepareWebRequest ()
        {
            if (url == null)
                throw new InvalidOperationException("You need to call one of the methods that set the request URL first");

            string queryString = queryParameters.AllKeys.Concat(x => "{0}={1}".Fmt(x, queryParameters[x]), "&");
            string requestUrl = string.IsNullOrEmpty(queryString) ? url : "{0}?{1}".Fmt(url, queryString);

            webRequest = (HttpWebRequest)WebRequest.Create(new Uri(requestUrl));
            webRequest.Method = method;
        }

        private bool disposed;
        private string url;
        private string method;
        private readonly NameValueCollection queryParameters = new NameValueCollection();
        private bool requestBodySet;
        private string requestString;
        private RestClientResponse response;
        private TimeSpan? timeout;
        private IWebConfiguration webConfiguration;
        private HttpWebRequest webRequest;
    }
}