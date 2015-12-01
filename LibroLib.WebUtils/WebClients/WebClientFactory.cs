using System;
using LibroLib.FileSystem;

namespace LibroLib.WebUtils.WebClients
{
    public class WebClientFactory : IWebClientFactory
    {
        public WebClientFactory(IFileSystem fileSystem, IWebConfiguration webConfiguration)
        {
            this.fileSystem = fileSystem;
            this.webConfiguration = webConfiguration;
        }

        public void Destroy(object component)
        {
            IDisposable disposable = component as IDisposable;

            if (disposable != null)
                disposable.Dispose();
        }

        public IWebClient CreateWebClient()
        {
            return new LibroLibWebClient(this, webConfiguration, fileSystem);
        }

        public WebClientEx CreateInnerWebClient()
        {
            return new WebClientEx(webConfiguration);
        }

        private readonly IWebConfiguration webConfiguration;
        private readonly IFileSystem fileSystem;
    }
}