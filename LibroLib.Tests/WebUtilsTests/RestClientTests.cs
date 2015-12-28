using System;
using System.Net;
using LibroLib.WebUtils;
using LibroLib.WebUtils.Rest;
using NUnit.Framework;

namespace LibroLib.Tests.WebUtilsTests
{
    [Category("integration")]
    public class RestClientTests
    {
        [Test]
        public void CreateClient()
        {
            using (restClientFactory.CreateRestClient())
            {
            }
        }

        [Test]
        public void SendSimpleGetRequest()
        {
            using (IRestClient client = restClientFactory.CreateRestClient ())
            {
                client.Get ("http://posttestserver.com/post.php").Do ();
                Assert.AreEqual ((int)HttpStatusCode.OK, client.StatusCode);
                StringAssert.StartsWith ("Successfully dumped 0 post variables", client.Response.AsString ());
            }
        }

        [Test]
        public void CheckBytesResponse()
        {
            using (IRestClient client = restClientFactory.CreateRestClient ())
            {
                client.Get ("http://posttestserver.com/post.php").Do ();
                Assert.AreEqual ((int)HttpStatusCode.OK, client.StatusCode);
                Assert.AreEqual(141, client.Response.AsBytes().Length);
            }
        }

        [Test, ExpectedException(typeof(RestException))]
        public void RequestThatTimeouts()
        {
            using (IRestClient client = restClientFactory.CreateRestClient ())
            {
                client
                    .Get ("http://posttestserver.com/post.php?sleep=5")
                    .WithTimeout(TimeSpan.FromMilliseconds(100)).Do ();
            }
        }

        [Test]
        public void CheckThatUserAgentIsSet()
        {
            using (IRestClient client = restClientFactory.CreateRestClient ())
            {
                client.Get ("http://google.com").Do ();
                Assert.AreEqual ((int)HttpStatusCode.OK, client.StatusCode);
                Assert.AreEqual (UserAgent, ((RestClient)client).NativeWebRequest.UserAgent);
            }
        }

        [Test]
        public void SendPostRequest()
        {
            using (IRestClient client = restClientFactory.CreateRestClient ())
            {
                client.Post ("http://posttestserver.com/post.php")
                    .Request("something")
                    .Do ();
                Assert.AreEqual((int)HttpStatusCode.OK, client.StatusCode);
                StringAssert.StartsWith ("Successfully dumped 0 post variables", client.Response.AsString ());
            }
        }

        [Test]
        public void EmptyPostRequest()
        {
            using (IRestClient client = restClientFactory.CreateRestClient ())
            {
                client.Post ("http://posttestserver.com/post.php")
                    .Do ();
                Assert.AreEqual((int)HttpStatusCode.OK, client.StatusCode);
                StringAssert.StartsWith ("Successfully dumped 0 post variables", client.Response.AsString ());
            }
        }

        [Test]
        public void CheckStatusCode()
        {
            using (IRestClient client = restClientFactory.CreateRestClient ())
            {
                client.Post ("http://posttestserver.com/post.php?status_code=201")
                    .Request("something")
                    .Do ();
                Assert.AreEqual(201, client.StatusCode);
            }
        }

        [Test]
        public void Status404Returned()
        {
            using (IRestClient client = restClientFactory.CreateRestClient ())
            {
                try
                {
                    client.Post ("http://posttestserver.com/post.php?status_code=404")
                        .Request ("something")
                        .Do ();
                }
                catch (RestException ex)
                {
                    Assert.AreEqual (404, ex.StatusCode);
                }
            }
        }

        [SetUp]
        public void Setup()
        {
            IWebConfiguration webConfiguration = new WebConfiguration(UserAgent);
            restClientFactory = new RestClientFactory(webConfiguration);
        }

        private IRestClientFactory restClientFactory;
        private const string UserAgent = "tests";
    }
}