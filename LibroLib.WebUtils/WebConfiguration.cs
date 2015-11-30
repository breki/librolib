using System.Net;

namespace LibroLib.WebUtils
{
    public class WebConfiguration : IWebConfiguration
    {
        public WebConfiguration(string userAgent)
        {
            this.userAgent = userAgent;
        }

        public string UserAgent
        {
            get { return userAgent; }
            set { userAgent = value; }
        }

        public bool UseProxy
        {
            get; set;
        }

        public string ProxyHost
        {
            get; set;
        }

        public int ProxyPort
        {
            get; set;
        }

        public ICredentials Credentials
        {
            get; set;
        }

        private string userAgent;
    }
}