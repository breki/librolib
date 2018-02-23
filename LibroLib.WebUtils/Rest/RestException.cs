using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using JetBrains.Annotations;

namespace LibroLib.WebUtils.Rest
{
    [SuppressMessage("Microsoft.Usage", "CA2240:ImplementISerializableCorrectly"), Serializable]
#pragma warning disable CA2229 // Implement serialization constructors
#pragma warning disable CA1032 // Implement standard exception constructors
#pragma warning disable RCS1194 // Implement exception constructors.
    public class RestException : Exception
#pragma warning restore RCS1194 // Implement exception constructors.
#pragma warning restore CA1032 // Implement standard exception constructors
#pragma warning restore CA2229 // Implement serialization constructors
    {
        // ReSharper disable once SuggestBaseTypeForParameter
        public RestException(
            string message,
            [NotNull] WebException innerException)
            : base(message, innerException)
        {
            WebException webException = (WebException)InnerException;

            if (webException == null)
                throw new InvalidOperationException("Something is wrong, InnerException should not be null.");

            if (webException.Response != null)
                statusCode = (int)((HttpWebResponse)webException.Response).StatusCode;

            webExceptionStatus = webException.Status;
        }

        public RestException(
            WebExceptionStatus webExceptionStatus,
            int? statusCode)
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

#pragma warning disable CA2235 // Mark all non-serializable fields
        private readonly int? statusCode;
#pragma warning restore CA2235 // Mark all non-serializable fields
        private readonly WebExceptionStatus webExceptionStatus;
    }
}