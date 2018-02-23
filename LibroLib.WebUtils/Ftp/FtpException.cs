using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization;

namespace LibroLib.WebUtils.Ftp
{
    [Serializable]
    public class FtpException : IOException
    {
        public FtpException()
        {
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "hresult")]
        public FtpException(string message, int hresult)
            : base(message, hresult)
        {
        }

        public FtpException(string message)
            : base(message)
        {
        }

        public FtpException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected FtpException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}