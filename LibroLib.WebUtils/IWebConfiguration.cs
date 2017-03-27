using System.Net;
using JetBrains.Annotations;

namespace LibroLib.WebUtils
{
    public interface IWebConfiguration
    {
        [CanBeNull]
        string UserAgent { get; }
        bool UseProxy { get; set; }
        [CanBeNull]
        string ProxyHost { get; set; }
        int ProxyPort { get; set; }
        [CanBeNull]
        ICredentials Credentials { get; }
    }
}