using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace LibroLib.WebUtils.Ftp
{
    public class FtpCommunicator : IFtpCommunicator
    {
        public void AttachToChannel(IFtpChannel channel)
        {
            this.channel = channel;
        }

        public void DetachFromChannel()
        {
            channel = null;
        }

        public FtpServerResponse ReadResponse()
        {
            string responseText = ReadResponseLine();

            if (responseText == null)
                throw new FtpException("responseText == null");

            if (singlelineResponseRegex.IsMatch(responseText))
                return new FtpServerResponse(responseText);

            if (!multilineResponseRegex.IsMatch(responseText))
                throw new FtpException();

            Contract.Assume(responseText.Length >= 4);

            FtpServerResponse firstResponseLine = new FtpServerResponse(responseText);

            while (true)
            {
                responseText = ReadResponseLine();

                if (responseText == null)
                    throw new FtpException();

                if (!singlelineResponseRegex.IsMatch(responseText))
                    continue;

                Contract.Assume(responseText.Length >= 4);

                FtpServerResponse response = new FtpServerResponse(responseText);
                if (response.ReturnCode == firstResponseLine.ReturnCode)
                    return firstResponseLine;
            }
        }

        public void SendCommand(string command)
        {
            //if (log.IsDebugEnabled)
            //    log.DebugFormat("Sending '{0}'", command);

            byte[] cmdBytes = Encoding.ASCII.GetBytes(command + "\r\n");
            int sentCount = channel.Send(cmdBytes);

            if (sentCount != cmdBytes.Length)
                throw new FtpException("Communication error");
        }

        private string ReadResponseLine()
        {
            if (responseLinesQueue.Count == 0)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    channel.Receive(stream);
                    string text = Encoding.ASCII.GetString(stream.ToArray());
                    if (text.Length == 0)
                        return null;

                    IList<string> lines = text.SplitIntoLines();
                    for (int i = 0; i < lines.Count; i++)
                        responseLinesQueue.Enqueue(lines[i]);
                }
            }

            string responseLine = responseLinesQueue.Dequeue();

            //if (log.IsDebugEnabled)
            //    log.DebugFormat("Response: '{0}'", responseLine);

            return responseLine;
        }

        private IFtpChannel channel;
        private static readonly Regex singlelineResponseRegex = new Regex(@"^[0-9]{3} ", RegexOptions.Compiled);
        private static readonly Regex multilineResponseRegex = new Regex(@"^[0-9]{3}\-", RegexOptions.Compiled);
        private readonly Queue<string> responseLinesQueue = new Queue<string>();
        //private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}