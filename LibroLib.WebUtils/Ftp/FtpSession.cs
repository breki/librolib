using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using LibroLib.FileSystem;
using LibroLib.Misc;

namespace LibroLib.WebUtils.Ftp
{
    public class FtpSession : IFtpSession
    {
        public FtpSession(
            IFtpChannelFactory ftpChannelFactory, 
            IFtpCommunicator ftpCommunicator,
            IFileSystem fileSystem)
        {
            Contract.Requires(ftpChannelFactory != null);
            Contract.Requires(ftpCommunicator != null);
            Contract.Requires(fileSystem != null);
            this.ftpChannelFactory = ftpChannelFactory;
            this.ftpCommunicator = ftpCommunicator;
            this.fileSystem = fileSystem;
        }

        public string CurrentDirectory
        {
            get { return currentDirectory; }
        }

        // ReSharper disable once ParameterHidesMember
        public void BeginSession(FtpConnectionData connectionData)
        {
            this.connectionData = connectionData;

            ftpChannel = ftpChannelFactory.CreateChannel();

            ftpChannel.Connect(connectionData.Host, connectionData.Port);
            ftpCommunicator.AttachToChannel(ftpChannel);

            FtpServerResponse response = ftpCommunicator.ReadResponse();
            if (response.ReturnCode != (int)FtpReturnCode.ServiceReadyForNewUser)
                throw FtpException("Could not connect to the FTP server", response);

            response = SendCommand("USER {0}", connectionData.Credentials.UserName);

            if (response.ReturnCode == (int)FtpReturnCode.UserNameOkNeedPassword)
                response = SendCommand("PASS {0}", connectionData.Credentials.Password);

            if (response.ReturnCode == (int)FtpReturnCode.UserLoggedIn)
            {
                SetBinaryMode();
                return;
            }

            throw FtpException("Could not log in to the FTP server", response);
        }

        public void ChangeDirectory(string directory)
        {
            FtpServerResponse response = SendCommand("CWD {0}", directory);
            if (response.ReturnCode != (int)FtpReturnCode.RequestedFileActionOkayCompleted)
                throw FtpException("Could not change the current directory", response);

            currentDirectory = directory;
        }

        public void CreateDirectory(string directory, bool failIfExists)
        {
            FtpServerResponse response = SendCommand("MKD {0}", directory);

            if (response.ReturnCode != (int)FtpReturnCode.Created)
            {
                if (!failIfExists && response.ReturnCode == (int)FtpReturnCode.RequestedActionNotTakenFileUnavailable)
                    return;

                throw FtpException("Could not create the directory", response);
            }
        }

        public void EndSession()
        {
            ftpCommunicator.DetachFromChannel();

            if (ftpChannel != null)
            {
                ftpChannel.Disconnect();
                ftpChannelFactory.Destroy(ftpChannel);
                ftpChannel = null;
            }
        }

        public void EnsurePathExists(
            string path, 
            HashSet<string> createdDirectories, 
            Action<string> beforeDirectoryCreatedCallback)
        {
            string parentPath = Path.GetDirectoryName(path);
            if (string.IsNullOrEmpty(parentPath))
                return;

            PathBuilder parentPathBuilder = new PathBuilder(parentPath);
            parentPath = parentPathBuilder.ToUnixPath();
            bool doesCreatedDirectoriesContainParentPath = createdDirectories != null && createdDirectories.Contains(parentPath);
            if (parentPath == "/" || doesCreatedDirectoriesContainParentPath)
                return;

            EnsurePathExists(parentPath, createdDirectories, beforeDirectoryCreatedCallback);

            if (beforeDirectoryCreatedCallback != null)
                beforeDirectoryCreatedCallback(parentPath);

            CreateDirectory(parentPath, false);

            if (createdDirectories != null)
                createdDirectories.Add(parentPath);
        }

        public IList<string> ListFiles(string directory)
        {
            OpenDataChannel();

            FtpServerResponse response = SendCommand("NLST {0}", directory);
            if (response.ReturnCode != (int)FtpReturnCode.DataConnectionAlreadyOpen
                && response.ReturnCode != (int)FtpReturnCode.FileStatusOk)
            {
                throw FtpException("Could not list files", response);
            }

            throw new NotImplementedException();
            //using (MemoryStream stream = new MemoryStream())
            //{
            //    dataChannel.Receive(stream);
            //    string text = Encoding.ASCII.GetString(stream.ToArray());
            //    return text.SplitIntoLines();
            //}
        }

        public void UploadFile(string localFileName, string remoteFileName)
        {
            EnsurePathExists(remoteFileName, null, null);
            UploadFileWithoutChecks(localFileName, remoteFileName);
        }

        public void UploadFiles(
            IExecutionContext taskExecutionContext,
            FileSet localFiles,
            string rootRemoteDirectory,
            Action<string> beforeDirectoryCreatedCallback,
            Action<string, string> beforeFileUploadedCallback)
        {
            HashSet<string> createdDirectories = new HashSet<string>();

            foreach (string localFileName in localFiles.Files)
            {
                if (taskExecutionContext != null && taskExecutionContext.ShouldAbort)
                    return;

                string debasedLocalFileName;

                if (localFiles.BaseDir != null)
                {
                    PathBuilder debasedPathBuilder = new PathBuilder (localFiles.BaseDir);
                    debasedLocalFileName = debasedPathBuilder.DebasePath (localFileName, true).ToString();
                }
                else
                    debasedLocalFileName = Path.GetFileName(localFileName);

                if (debasedLocalFileName == null)
                    throw new InvalidOperationException("debasedLocalFileName == null");

                PathBuilder remoteFileNameBuilder =
                    new PathBuilder(Path.Combine(rootRemoteDirectory, debasedLocalFileName));
                string remoteFileName = remoteFileNameBuilder.ToUnixPath();
                EnsurePathExists(remoteFileName, createdDirectories, beforeDirectoryCreatedCallback);

                if (beforeFileUploadedCallback != null)
                    beforeFileUploadedCallback(localFileName, remoteFileName);

                UploadFileWithoutChecks(localFileName, remoteFileName);
            }
        }

        public void UploadFileWithoutChecks(string localFileName, string remoteFileName)
        {
            Contract.Requires(localFileName != null);
            Contract.Requires(remoteFileName != null);

            FtpServerResponse response;
            IFtpChannel dataChannel = null;

            try
            {
                dataChannel = OpenDataChannel();

                response = SendCommand("STOR {0}", remoteFileName);
                if (response.ReturnCode != (int)FtpReturnCode.DataConnectionAlreadyOpen
                    && response.ReturnCode != (int)FtpReturnCode.FileStatusOk)
                {
                    throw FtpException("Could not upload file", response);
                }

                byte[] data = fileSystem.ReadFileAsBytes(localFileName);
                if (dataChannel.Send(data) != data.Length)
                    throw new FtpException("Not all of file was uploaded");
            }
            finally
            {
                if (dataChannel != null)
                {
                    dataChannel.Disconnect();
                    ftpChannelFactory.Destroy(dataChannel);
                }
            }

            response = ftpCommunicator.ReadResponse();
            if (response.ReturnCode != (int)FtpReturnCode.RequestedFileActionOkayCompleted
                && response.ReturnCode != (int)FtpReturnCode.ClosingDataConnection)
            {
                throw FtpException("Could not upload file", response);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposed)
                return;

            // clean native resources         

            if (disposing)
            {
                // clean managed resources  
                EndSession();
            }

            disposed = true;
        }

        private static FtpException FtpException(string errorMessage, FtpServerResponse response)
        {
            Contract.Requires(errorMessage != null);
            Contract.Requires(response != null);
            string message = string.Format(
                CultureInfo.InvariantCulture,
                "{0}: {1}",
                errorMessage,
                response);
            return new FtpException(message, response.ReturnCode);            
        }

        private FtpServerResponse SendCommand(string commandFormat, params object[] args)
        {
            Contract.Requires(commandFormat != null);
            Contract.Requires(args != null);

            string command = string.Format(
                CultureInfo.InvariantCulture,
                commandFormat,
                args);
            ftpCommunicator.SendCommand(command);
            return ftpCommunicator.ReadResponse();
        }

        private void SetBinaryMode()
        {
            FtpServerResponse response = SendCommand("TYPE I");
            if (response.ReturnCode != (int)FtpReturnCode.CommandOk)
                throw FtpException("Could not execute command", response);
        }

        private IFtpChannel OpenDataChannel()
        {
            FtpServerResponse response = SendCommand("PASV");
            if (response.ReturnCode != (int)FtpReturnCode.EnteringPassiveMode)
                throw FtpException("Could not upload file", response);

            string responseMessage = response.Message;
            int openBracketIndex = responseMessage.IndexOf('(');

            if (openBracketIndex < 0)
                throw new FtpException("Unexpected response message: '{0}'".Fmt(responseMessage));

            int start = openBracketIndex + 1;
            int closeBracketIndex = responseMessage.IndexOf(')', start);
            if (closeBracketIndex < 0)
                throw new FtpException ("Unexpected response message: '{0}'".Fmt (responseMessage));

            int length = closeBracketIndex - start;
            string[] splits = responseMessage.Substring(start, length).Split(',');

            Contract.Assert(splits.Length >= 6);

            byte[] addressBytes = new byte[4];

            for (int i = 0; i < 4; i++)
                addressBytes[i] = byte.Parse(splits[i], CultureInfo.InvariantCulture);

            int dataChannelPort = int.Parse(splits[4], CultureInfo.InvariantCulture) * 256
                                  + int.Parse(splits[5], CultureInfo.InvariantCulture);
            IFtpChannel dataChannel = ftpChannelFactory.CreateChannel();
            dataChannel.Connect(addressBytes, dataChannelPort);
            return dataChannel;
        }

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        private FtpConnectionData connectionData;
        private bool disposed;
        private string currentDirectory;
        private IFtpChannel ftpChannel;
        private readonly IFileSystem fileSystem;
        private readonly IFtpChannelFactory ftpChannelFactory;
        private readonly IFtpCommunicator ftpCommunicator;
    }
}