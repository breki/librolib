using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net;
using System.Xml.Serialization;
using JetBrains.Annotations;
using LibroLib.FileSystem;
using LibroLib.Misc;

namespace LibroLib.WebUtils.WebClients
{
    public class LibroLibWebClient : IWebClient
    {
        public LibroLibWebClient(
            IWebClientFactory webClientFactory,
            IWebConfiguration configuration,
            IFileSystem fileSystem)
        {
            this.webClientFactory = webClientFactory;
            this.configuration = configuration;
            this.fileSystem = fileSystem;
        }

        public Action<bool, string> LogAction { get; set; }

        public void DownloadFile(Uri url, TimeSpan timeout, string localPath)
        {
            using (FactoryLease<WebClientEx> webClient =
                new FactoryLease<WebClientEx>(webClientFactory.CreateInnerWebClient(), webClientFactory))
            {
                ConfigureWebClient(webClient.Obj, timeout);

                try
                {
                    fileSystem.EnsureDirectoryExists(Path.GetDirectoryName(localPath));
                    webClient.Obj.DownloadFile(url, localPath);
                }
                catch (WebException ex)
                {
                    Log(
                        true,
                        "DownloadFile (url='{0}') failed. Reason : {1}",
                        url,
                        ex);
                    throw;
                }
            }
        }

        public T DownloadObject<T>(Uri url, TimeSpan timeout)
        {
            T value = default(T);

            DownloadData(
                url,
                timeout,
                s =>
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    value = (T)serializer.Deserialize(s);
                });

            return value;
        }

        public string DownloadString(Uri url, TimeSpan timeout)
        {
            using (FactoryLease<WebClientEx> webClient =
                new FactoryLease<WebClientEx>(webClientFactory.CreateInnerWebClient(), webClientFactory))
            {
                ConfigureWebClient(webClient.Obj, timeout);

                try
                {
                    return webClient.Obj.DownloadString(url);
                }
                catch (WebException ex)
                {
                    Log (
                        true,
                        "DownloadString (url='{0}') failed. Reason : {1}",
                        url,
                        ex);
                    throw;
                }
            }
        }

        public void DownloadData(Uri url, TimeSpan timeout, [NotNull] Action<Stream> streamAction)
        {
            if (streamAction == null)
                throw new ArgumentNullException(nameof(streamAction));

            WebRequest request = PrepareRequest(url, timeout);

            Log(false, "Sending the request to URL '{0}'", url);

            using (WebResponse response = request.GetResponse())
            {
                try
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        Log(false, "Received response stream from URL '{0}'", url);

                        streamAction(stream);
                    }

                    Log(false, "Finishing downloading data from URL '{0}'", url);
                }
                catch (WebException ex)
                {
                    Log (
                        true,
                        "DownloadData (url='{0}') failed. Reason : {1}",
                        url,
                        ex);
                    throw;
                }
            }
        }

        public byte[] DownloadData(Uri url, TimeSpan timeout)
        {
            using (FactoryLease<WebClientEx> webClient =
                new FactoryLease<WebClientEx>(webClientFactory.CreateInnerWebClient(), webClientFactory))
            {
                ConfigureWebClient(webClient.Obj, timeout);

                try
                {
                    return webClient.Obj.DownloadData(url);
                }
                catch (WebException ex)
                {
                    Log (
                        true,
                        "DownloadData (url='{0}') failed. Reason : {1}",
                        url,
                        ex);
                    throw;
                }
            }
        }

        public bool IsInternetAccessAvailable(TimeSpan timeout)
        {
            // TODO: ping should be replaced with something that can go through web proxies
            return true;
        }

        public byte[] UploadData(Uri url, byte[] data, TimeSpan timeout)
        {
            using (FactoryLease<WebClientEx> webClient =
                new FactoryLease<WebClientEx>(webClientFactory.CreateInnerWebClient(), webClientFactory))
            {
                ConfigureWebClient(webClient.Obj, timeout);

                try
                {
                    return webClient.Obj.UploadData(url, data);
                }
                catch (WebException ex)
                {
                    Log (
                        true,
                        "UploadData (url='{0}') failed. Reason : {1}",
                        url,
                        ex);
                    throw;
                }
            }
        }

        public byte[] UploadData(Uri url, string method, byte[] data, TimeSpan timeout)
        {
            using (FactoryLease<WebClientEx> webClient =
                new FactoryLease<WebClientEx>(webClientFactory.CreateInnerWebClient(), webClientFactory))
            {
                ConfigureWebClient(webClient.Obj, timeout);

                try
                {
                    return webClient.Obj.UploadData(url, method, data);
                }
                catch (WebException ex)
                {
                    Log (
                        true,
                        "UploadData (url='{0}') failed. Reason : {1}",
                        url,
                        ex);
                    throw;
                }
            }
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
            }

            disposed = true;
        }

        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        private WebRequest PrepareRequest(Uri url, TimeSpan timeout)
        {
            Contract.Requires(url != null);

            Log(false, "Preparing request for URL '{0}', timeout = {1}", url, timeout);

            WebRequest request = WebRequest.Create(url);
            request.Timeout = (int)timeout.TotalMilliseconds;

            if (request is HttpWebRequest)
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)request;
                httpWebRequest.UserAgent = configuration.UserAgent;
            }

            if (configuration.UseProxy)
                request.Proxy = ConfigureWebProxySettings();

            return request;
        }

        private void ConfigureWebClient(WebClientEx webClient, TimeSpan timeout)
        {
            Contract.Requires(webClient != null);

            webClient.SetTimeout(timeout);

            if (configuration.UseProxy)
            {
                webClient.Proxy = ConfigureWebProxySettings();
            }
            else
            {
                // this is to prevent proxy auto-detection which slows down first HTTP request considerably
                // (10-15 seconds)
                webClient.Proxy = null;
            }
        }

        private WebProxy ConfigureWebProxySettings()
        {
            Log (
                false,
                "Configuring HTTP proxy to '{0}:{1}'",
                configuration.ProxyHost,
                configuration.ProxyPort);

            WebProxy webProxy = new WebProxy(configuration.ProxyHost, configuration.ProxyPort);
            webProxy.Credentials = configuration.Credentials;
            return webProxy;
        }

        [StringFormatMethod("format")]
        private void Log(bool isError, string format, params object[] args)
        {
            if (LogAction == null)
                return;

            LogAction(isError, format.Fmt(args));
        }

        private readonly IWebConfiguration configuration;
        private readonly IFileSystem fileSystem;
        private bool disposed;
        private readonly IWebClientFactory webClientFactory;
    }
}