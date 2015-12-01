using LibroLib.Misc;

namespace LibroLib.WebUtils.WebClients
{
    public interface IWebClientFactory : IFactory
    {
        IWebClient CreateWebClient();
        WebClientEx CreateInnerWebClient();
    }
}