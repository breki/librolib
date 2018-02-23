using System;
using System.Net;
using LibroLib.FileSystem;
using LibroLib.WebUtils.Ftp;
using NUnit.Framework;

namespace LibroLib.Tests.WebUtilsTests.FtpTests
{
    [Explicit("You need a FTP server to run these tests")]
    public class RealFtpTests
    {
        [Test]
        public void InvalidUser()
        {
            FtpConnectionData connectionData = new FtpConnectionData();
            connectionData.Credentials = new NetworkCredential(Username + "xxx", Password);
            connectionData.Host = Host;

            using (IFtpSession session
                = new FtpSession(new FtpChannelFactoryUsingSockets(), new FtpCommunicator(), new WindowsFileSystem()))
            {
                var ex = Assert.Throws<FtpException>(delegate { session.BeginSession(connectionData); });
                Assert.AreEqual("Could not log in to the FTP server: 530 User cannot log in.", ex.Message);
            }
        }

        [Test]
        public void InvalidPassword()
        {
            FtpConnectionData connectionData = new FtpConnectionData();
            connectionData.Credentials = new NetworkCredential(Username, Password + "xxx");
            connectionData.Host = Host;

            using (IFtpSession session
                = new FtpSession(new FtpChannelFactoryUsingSockets(), new FtpCommunicator(), new WindowsFileSystem()))
            {
                var ex = Assert.Throws<FtpException>(delegate { session.BeginSession(connectionData); });
                Assert.AreEqual("Could not log in to the FTP server: 530 User cannot log in.", ex.Message);
            }
        }

        [Test]
        public void LoginOK()
        {
            FtpAction(c => { });
        }

        [Test]
        public void TryToCreateDirectoryThatAlreadyExists()
        {
            var ex = Assert.Throws<FtpException>(delegate { FtpAction(c => c.CreateDirectory("test", true)); });
            Assert.AreEqual("Could not create the directory: 550 Cannot create a file when that file already exists.", ex.Message);
        }

        [Test]
        public void TryToCreateDirectorySubtree()
        {
            var ex = Assert.Throws<FtpException>(delegate { FtpAction(c => c.CreateDirectory("test2/test3/test4", true)); });
            Assert.AreEqual("Could not create the directory: 550 The system cannot find the path specified.", ex.Message);
        }

        [Test]
        public void UploadFile()
        {
            FtpAction(c => c.UploadFile(Constants.DataSamplesPath + @"Bitmaps/cea.tif", "cea.tif"));
        }

        [Test]
        public void UploadFileToANewDir()
        {
            FtpAction(c => c.UploadFile(Constants.DataSamplesPath + @"Bitmaps/cea.tif", "test1/test2/test3/cea.tif"));
        }

        [Test]
        public void UploadFiles()
        {
            string[] files =
                {
                    @"D:\MyStuff\projects\Maperitive\trunk\bin\Debug\Maperitive\Tiles\13\4448\2895.png",
                    @"D:\MyStuff\projects\Maperitive\trunk\bin\Debug\Maperitive\Tiles\13\4449\2895.png"
                };
            FileSet fileSet = new FileSet(@"D:\MyStuff\projects\Maperitive\trunk\bin\Debug\Maperitive\Tiles", files);

            FtpAction(c => c.UploadFiles(null, fileSet, "tiles2", null, null));
        }

        private static void FtpAction(Action<IFtpSession> action)
        {
            FtpConnectionData connectionData = new FtpConnectionData();
            connectionData.Credentials = new NetworkCredential(Username, Password);
            connectionData.Host = Host;
            connectionData.Port = Port;

            using (IFtpSession session
                = new FtpSession(new FtpChannelFactoryUsingSockets(), new FtpCommunicator(), new WindowsFileSystem()))
            {
                session.BeginSession(connectionData);
                action(session);
            }
        }

        private const string Host = "127.0.0.1";
        private const int Port = 21;
        private const string RemoteDir = "MaperitiveTests";
        private const string Username = @"blabla";
        private const string Password = "blabla2";
    }
}