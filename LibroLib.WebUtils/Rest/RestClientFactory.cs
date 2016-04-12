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
#pragma warning disable CC0022 // Should dispose object
            return new RestClient().WithConfiguration(webConfiguration);
#pragma warning restore CC0022 // Should dispose object
        }

        private readonly IWebConfiguration webConfiguration;
    }
}