using System;
using System.IO;
using System.Text;
using LibroLib.WebUtils.Ftp;
using NUnit.Framework;
using Rhino.Mocks;

namespace LibroLib.Tests.WebUtilsTests.FtpTests
{
    public class FtpCommunicatorTests
    {
        [Test]
        public void ReadSingleLineResponse()
        {
            StubChannelResponse("200 bla\r\n");

            comm.AttachToChannel(channel);
            FtpServerResponse response = comm.ReadResponse();
            Assert.AreEqual((int)FtpReturnCode.CommandOk, response.ReturnCode);
            Assert.AreEqual("bla", response.Message);
        }

        [Test]
        public void ReadMultiLineResponse()
        {
            StubChannelResponse("200-bla\r\n");
            StubChannelResponse(" 300 test\r\n");
            StubChannelResponse("200 OK\r\n");

            comm.AttachToChannel(channel);
            FtpServerResponse response = comm.ReadResponse();
            Assert.AreEqual((int)FtpReturnCode.CommandOk, response.ReturnCode);
            Assert.AreEqual("bla", response.Message);
        }

        [Test]
        public void ReadMultiLineResponseInBatch()
        {
            StubChannelResponse("200-bla\r\n 300 test\r\n200 OK\r\n");

            comm.AttachToChannel(channel);
            FtpServerResponse response = comm.ReadResponse();
            Assert.AreEqual((int)FtpReturnCode.CommandOk, response.ReturnCode);
            Assert.AreEqual("bla", response.Message);
        }

        [Test]
        public void ReadInvalidSingleLineResponse()
        {
            StubChannelResponse("bla\r\n");

            comm.AttachToChannel(channel);

            Assert.Throws<FtpException>(delegate { comm.ReadResponse(); });
        }

        [Test]
        public void ReadInvalidMultiLineResponse()
        {
            StubChannelResponse("200-bla\r\n");
            StubChannelResponse(" 300 test\r\n");

            comm.AttachToChannel(channel);
            Assert.Throws<FtpException>(delegate { comm.ReadResponse (); });
        }

        [SetUp]
        public void Setup()
        {
            channel = MockRepository.GenerateMock<IFtpChannel>();
            comm = new FtpCommunicator();
        }

        private void StubChannelResponse (string text)
        {
            channel.Stub(x => x.Receive(null))
                .IgnoreArguments()
                .Do(new Action<Stream>(s => WriteToStream(s, text)))
                .Repeat.Once();
        }

        private static void WriteToStream(Stream stream, string text)
        {
            byte[] data = Encoding.ASCII.GetBytes(text);
            stream.Write(data, 0, data.Length);
        }

        private IFtpChannel channel;
        private IFtpCommunicator comm;
    }
}