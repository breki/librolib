using System.Diagnostics.Contracts;
using System.Globalization;

namespace LibroLib.WebUtils.Ftp
{
    public class FtpServerResponse
    {
        public FtpServerResponse(string fullMessage)
        {
            Contract.Requires(fullMessage != null);
            Contract.Requires (fullMessage.Length >= 4);

            returnCode = ParseResponseCode(fullMessage);
            message = fullMessage.Substring(4).Trim();
        }

        public FtpServerResponse(int returnCode, string message)
        {
            Contract.Requires(message != null);

            this.returnCode = returnCode;
            this.message = message;
        }

        public FtpServerResponse(FtpReturnCode returnCode, string message)
        {
            Contract.Requires(message != null);

            this.returnCode = (int)returnCode;
            this.message = message;
        }

        public int ReturnCode
        {
            get { return returnCode; }
        }

        public string Message
        {
            get { return message; }
        }

        public static int ParseResponseCode (string fullMessage)
        {
            Contract.Requires(fullMessage != null);
            Contract.Requires (fullMessage.Length >= 3);

            return int.Parse(fullMessage.Substring(0, 3), NumberStyles.Integer, CultureInfo.InvariantCulture);
        }

        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0} {1}",
                returnCode,
                message);
        }

        private readonly int returnCode;
        private readonly string message;
    }
}