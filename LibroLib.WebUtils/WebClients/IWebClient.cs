using System;
using System.IO;

namespace LibroLib.WebUtils.WebClients
{
    public interface IWebClient : IDisposable
    {
        void DownloadData (Uri url, TimeSpan timeout, Action<Stream> streamAction);
        byte[] DownloadData (Uri url, TimeSpan timeout);
        void DownloadFile (Uri url, TimeSpan timeout, string localPath);
        T DownloadObject<T>(Uri url, TimeSpan timeout);
        string DownloadString (Uri url, TimeSpan timeout);
        bool IsInternetAccessAvailable(TimeSpan timeout);
        byte[] UploadData (Uri url, byte[] data, TimeSpan timeout);
        byte[] UploadData (Uri url, string method, byte[] data, TimeSpan timeout);
    }
}