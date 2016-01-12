using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using LibroLib.FileSystem;
using LibroLib.Misc;

namespace LibroLib.WebUtils.Ftp
{
    [ContractClass(typeof(IFtpSessionContract))]
    public interface IFtpSession : IDisposable
    {
        void BeginSession(FtpConnectionData connectionData);
        //void ChangeDirectory(string directory);
        void CreateDirectory(string directory, bool failIfExists);
        //bool DirectoryExists(string directory);
        void EndSession();
        //IList<string> ListFiles(string directory);

        void EnsurePathExists(
            string path, 
            HashSet<string> createdDirectories, 
            Action<string> beforeDirectoryCreatedCallback);

        void UploadFile(string localFileName, string remoteFileName);

        void UploadFiles(
            IExecutionContext taskExecutionContext,
            FileSet localFiles,
            string rootRemoteDirectory,
            Action<string> beforeDirectoryCreatedCallback,
            Action<string, string> beforeFileUploadedCallback);
    }

    [ContractClassFor(typeof(IFtpSession))]
    internal abstract class IFtpSessionContract : IFtpSession
    {
        void IDisposable.Dispose()
        {
        }

        void IFtpSession.BeginSession(FtpConnectionData connectionData)
        {
            Contract.Requires(connectionData != null);
        }

        void IFtpSession.CreateDirectory(string directory, bool failIfExists)
        {
            Contract.Requires(directory != null);
        }

        void IFtpSession.EndSession()
        {
        }

        void IFtpSession.EnsurePathExists(string path, HashSet<string> createdDirectories, Action<string> beforeDirectoryCreatedCallback)
        {
            Contract.Requires(path != null);
        }

        void IFtpSession.UploadFile(string localFileName, string remoteFileName)
        {
            Contract.Requires(localFileName != null);
            Contract.Requires(remoteFileName != null);
        }

        void IFtpSession.UploadFiles(IExecutionContext taskExecutionContext, FileSet localFiles, string rootRemoteDirectory, Action<string> beforeDirectoryCreatedCallback, Action<string, string> beforeFileUploadedCallback)
        {
            Contract.Requires(localFiles != null);
            Contract.Requires(rootRemoteDirectory != null);
        }
    }
}