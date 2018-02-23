using System;
using System.Collections.Generic;
using System.Net;
using LibroLib.FileSystem;
using LibroLib.WebUtils.Ftp;
using Moq;
using NUnit.Framework;

namespace LibroLib.Tests.WebUtilsTests.FtpTests
{
    [TestFixture]
    public class FtpSessionTests
    {
        [Test]
        public void LoginWithUserNameOnly()
        {
            SetResponse(FtpReturnCode.ServiceReadyForNewUser);
            ExpectCommand("USER banana");
            SetResponse(FtpReturnCode.UserLoggedIn);
            ExpectCommand("TYPE I");
            SetResponse(FtpReturnCode.CommandOk);

            FtpConnectionData connectionData = ConstructConnectionData();
            session.BeginSession(connectionData);

            communicator.VerifyAll();
        }

        [Test]
        public void LoginWithUserNameAndPassword()
        {
            SetLoginExpectations();

            FtpConnectionData connectionData = ConstructConnectionData();
            session.BeginSession(connectionData);

            communicator.VerifyAll();
        }

        [Test]
        public void ChangeDirectory()
        {
            const string RemoteDir = "directory1/directory2";

            SetLoginExpectations();
            ExpectCommand("CWD " + RemoteDir);
            SetResponse(FtpReturnCode.RequestedFileActionOkayCompleted);

            FtpConnectionData connectionData = ConstructConnectionData();
            session.BeginSession(connectionData);
            session.ChangeDirectory(RemoteDir);
            Assert.AreEqual(RemoteDir, session.CurrentDirectory);

            communicator.VerifyAll();
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void UploadFiles(bool withBaseDir)
        {
            SetLoginExpectations();
            ExpectCommand("MKD /dest");
            SetResponse(FtpReturnCode.Created);
            ExpectCommand("MKD /dest/dest2");
            SetResponse(FtpReturnCode.Created);
            ExpectCommand("PASV");
            SetResponse(FtpReturnCode.EnteringPassiveMode, "(10,20,30,40,1,2)");
            dataChannel.Setup(x => x.Connect(new byte[] { 10, 20, 30, 40 }, 258));
            ExpectCommand("STOR /dest/dest2/file1.txt");
            SetResponse(FtpReturnCode.FileStatusOk);
            fileSystem.Setup(x => x.ReadFileAsBytes(@"d:\files\file1.txt"))
                .Returns(new byte[10]);
            dataChannel.Setup(x => x.Send(It.IsAny<byte[]>())).Returns(10);
            SetResponse(FtpReturnCode.ClosingDataConnection);

            //ExpectCommand("PASV");
            SetResponse(FtpReturnCode.EnteringPassiveMode, "(10,20,30,40,1,2)");
            dataChannel.Setup(x => x.Connect(new byte[] { 10, 20, 30, 40 }, 258));
            ExpectCommand("STOR /dest/dest2/file2.txt");
            SetResponse(FtpReturnCode.FileStatusOk);
            fileSystem.Setup(x => x.ReadFileAsBytes(@"d:\files\file2.txt"))
                .Returns(new byte[10]);
            SetResponse(FtpReturnCode.ClosingDataConnection);

            FtpConnectionData connectionData = ConstructConnectionData();
            session.BeginSession(connectionData);

            List<string> createdDirs = new List<string>();
            List<KeyValuePair<string, string>> uploadedFiles = new List<KeyValuePair<string, string>>();
            FileSet files = new FileSet(withBaseDir ? @"d:\files" : null, new[] { @"d:\files\file1.txt", @"d:\files\file2.txt" });
            session.UploadFiles(
                null,
                files,
                "/dest/dest2",
                createdDirs.Add,
                (l, r) => uploadedFiles.Add(new KeyValuePair<string, string>(l, r)));

            Assert.AreEqual(2, createdDirs.Count);
            Assert.AreEqual(2, uploadedFiles.Count);

            communicator.VerifyAll();
        }

        [SetUp]
        public void Setup()
        {
            channelCounter = 0;
            mainChannel = new Mock<IFtpChannel>();
            dataChannel = new Mock<IFtpChannel>();

            channelFactory = new Mock<IFtpChannelFactory>();
            channelFactory.Setup(x => x.CreateChannel())
                .Callback(new Func<IFtpChannel>(CreateChannel));

            communicator = new Mock<IFtpCommunicator>();
            fileSystem = new Mock<IFileSystem>();
            session = new FtpSession(
                channelFactory.Object, communicator.Object, fileSystem.Object);
        }

        private static FtpConnectionData ConstructConnectionData()
        {
            FtpConnectionData connectionData = new FtpConnectionData();
            connectionData.Credentials = new NetworkCredential(Username, Password);
            connectionData.Host = Host;
            connectionData.Port = Port;
            return connectionData;
        }

        private void SetLoginExpectations()
        {
            SetResponse(FtpReturnCode.ServiceReadyForNewUser);
            ExpectCommand("USER banana");
            SetResponse(FtpReturnCode.UserNameOkNeedPassword);
            ExpectCommand("PASS njam");
            SetResponse(FtpReturnCode.UserLoggedIn);
            ExpectCommand("TYPE I");
            SetResponse(FtpReturnCode.CommandOk);
        }

        private void SetResponse(FtpReturnCode returnCode)
        {
            communicator.Setup(x => x.ReadResponse())
                .Returns(new FtpServerResponse(returnCode, "x"));
        }

        private void SetResponse(FtpReturnCode returnCode, string message)
        {
            communicator.Setup(x => x.ReadResponse())
                .Returns(new FtpServerResponse(returnCode, message));
        }

        private void ExpectCommand (string command)
        {
            communicator.Setup(x => x.SendCommand(command));
        }

        private IFtpChannel CreateChannel()
        {
            switch (channelCounter++)
            {
                case 0:
                    return mainChannel.Object;
                default:
                    return dataChannel.Object;
            }
        }

        private Mock<IFtpChannelFactory> channelFactory;
        private Mock<IFtpChannel> mainChannel;
        private Mock<IFtpChannel> dataChannel;
        private int channelCounter;
        private Mock<IFtpCommunicator> communicator;
        private Mock<IFileSystem> fileSystem;
        private FtpSession session;

        private const string Host = "somehost";
        private const int Port = 21;
        private const string Username = "banana";
        private const string Password = "njam";
    }
}