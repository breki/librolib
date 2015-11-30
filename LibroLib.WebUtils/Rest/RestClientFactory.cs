namespace LibroLib.WebUtils.Rest
{
    public class RestClientFactory : IRestClientFactory
    {
        public RestClientFactory(IWebConfiguration webConfiguration)
        {
            this.webConfiguration = webConfiguration;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage ("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public IRestClient CreateRestClient()
        {
            return new RestClient().WithConfiguration(webConfiguration);
        }

        private readonly IWebConfiguration webConfiguration;
    }
}