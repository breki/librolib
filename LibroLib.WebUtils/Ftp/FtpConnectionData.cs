using System.Net;

namespace LibroLib.WebUtils.Ftp
{
    public class FtpConnectionData
    {
        public NetworkCredential Credentials { get; set; }
        public string Host { get; set; }
        public int? Port { get; set; }
    }
}