using System;
using System.Net;

namespace LibroLib.WebUtils.Rest
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage ("Microsoft.Usage", "CA2240:ImplementISerializableCorrectly"), Serializable]
    public class RestException : Exception
    {
        public RestException(string message, WebException innerException) : base(message, innerException)
        {
            WebException webException = (WebException)InnerException;

            if (webException.Response != null)
                statusCode = (int)((HttpWebResponse)webException.Response).StatusCode;

            webExceptionStatus = webException.Status;
        }

        public RestException(WebExceptionStatus webExceptionStatus, int? statusCode)
        {
            this.webExceptionStatus = webExceptionStatus;
            this.statusCode = statusCode;
        }

        public int? StatusCode
        {
            get { return statusCode; }
        }

        public WebExceptionStatus WebExceptionStatus
        {
            get { return webExceptionStatus; }
        }

        private readonly int? statusCode;
        private readonly WebExceptionStatus webExceptionStatus;
    }
}