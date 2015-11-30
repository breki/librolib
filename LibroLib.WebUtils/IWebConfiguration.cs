using System.Net;

namespace LibroLib.WebUtils
{
    public interface IWebConfiguration
    {
        string UserAgent { get; }
        bool UseProxy { get; set; }
        string ProxyHost { get; set; }
        int ProxyPort { get; set; }
        ICredentials Credentials { get; }
    }
}