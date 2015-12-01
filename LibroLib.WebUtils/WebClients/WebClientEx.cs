using System;
using System.Diagnostics.Contracts;
using System.Net;

namespace LibroLib.WebUtils.WebClients
{
    public class WebClientEx : WebClient
    {
        public WebClientEx (IWebConfiguration webConfiguration)
        {
            Contract.Requires(webConfiguration != null);

            this.webConfiguration = webConfiguration;
        }

        public void SetTimeout(TimeSpan timeout)
        {
            this.timeout = timeout;
        }

        protected override WebRequest GetWebRequest (Uri address)
        {
            Headers["User-Agent"] = webConfiguration.UserAgent;
            Credentials = webConfiguration.Credentials;

            WebRequest webRequest = base.GetWebRequest (address);
            if (timeout != TimeSpan.Zero)
                webRequest.Timeout = (int)timeout.TotalMilliseconds;
            return webRequest;
        }

        private readonly IWebConfiguration webConfiguration;
        private TimeSpan timeout = TimeSpan.FromSeconds(20);
    }
}