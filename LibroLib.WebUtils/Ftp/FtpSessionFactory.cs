using System.Diagnostics.Contracts;
using LibroLib.FileSystem;

namespace LibroLib.WebUtils.Ftp
{
    public class FtpSessionFactory : IFtpSessionFactory
    {
        public FtpSessionFactory(IFtpChannelFactory ftpChannelFactory, IFtpCommunicator ftpCommunicator, IFileSystem fileSystem)
        {
            Contract.Requires(ftpChannelFactory != null);
            Contract.Requires(ftpCommunicator != null);
            Contract.Requires(fileSystem != null);
            this.ftpChannelFactory = ftpChannelFactory;
            this.ftpCommunicator = ftpCommunicator;
            this.fileSystem = fileSystem;
        }

        public void Destroy(object component)
        {
            if (component == null)
                return;

            ((IFtpSession)component).Dispose();
        }

        public IFtpSession CreateSession()
        {
            return new FtpSession(ftpChannelFactory, ftpCommunicator, fileSystem);
        }

        private readonly IFtpChannelFactory ftpChannelFactory;
        private readonly IFtpCommunicator ftpCommunicator;
        private readonly IFileSystem fileSystem;
    }
}