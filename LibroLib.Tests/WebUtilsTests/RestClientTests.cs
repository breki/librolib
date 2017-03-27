using System;
using System.IO;
using System.Net;
using System.Text;
using LibroLib.WebUtils;
using LibroLib.WebUtils.Rest;
using Newtonsoft.Json.Linq;
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
                byte[] responseBytes = client.Response.AsBytes();
                StringAssert.StartsWith ("Successfully dumped 0 post variables", Encoding.ASCII.GetString (responseBytes));
            }
        }

        [Test]
        public void CheckResponseHeaders()
        {
            using (IRestClient client = restClientFactory.CreateRestClient ())
            {
                client.Get ("http://posttestserver.com/post.php").Do ();
                Assert.AreEqual ((int)HttpStatusCode.OK, client.StatusCode);
                Assert.GreaterOrEqual(client.Response.Headers.Count, 1);
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
        public void SendSomeHeaders()
        {
            using (IRestClient client = restClientFactory.CreateRestClient ())
            {
                client.Get ("http://google.com").AddHeader("test-head", "test-value").Do ();
                Assert.AreEqual ((int)HttpStatusCode.OK, client.StatusCode);
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
        public void SendPostRequestAndUseResponseStream()
        {
            using (IRestClient client = restClientFactory.CreateRestClient ())
            {
                client.Post ("http://posttestserver.com/post.php")
                    .Request("something")
                    .Do ();
                Assert.AreEqual((int)HttpStatusCode.OK, client.StatusCode);

                string responseText;
                using (StreamReader reader = new StreamReader(client.Response.AsStream()))
                    responseText = reader.ReadToEnd();

                StringAssert.StartsWith ("Successfully dumped 0 post variables", responseText);
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

        [Test]
        public void HeadRequest()
        {
            using (IRestClient client = restClientFactory.CreateRestClient ())
            {
                var reponse = client.Head("http://posttestserver.com/post.php").Do().Response;
                Assert.IsNotNull(reponse);
            }
        }

        [Test]
        public void BasicAuthenticationWithCorrectCredentials()
        {
            using (IRestClient client = restClientFactory.CreateRestClient ())
            {
                const string Username = "user";
                const string Password = "crocodile";
                client.Credentials(new NetworkCredential(Username, Password));

                var reponse = client.Get (
                    "http://httpbin.org/basic-auth/{0}/{1}".Fmt(Username, Password)).Do ().Response;
                Assert.IsNotNull (reponse);
            }
        }

        [Test]
        public void BasicAuthenticationWithIncorrectCredentials()
        {
            using (IRestClient client = restClientFactory.CreateRestClient ())
            {
                const string Username = "user";
                const string Password = "crocodile";
                client.Credentials(new NetworkCredential(Username, Password));

                RestException ex = Assert.Throws<RestException>(
                    () => client.Get("http://httpbin.org/basic-auth/{0}/{1}".Fmt(Username, "hamster")).Do());
                Assert.AreEqual((int)HttpStatusCode.Unauthorized, ex.StatusCode);
            }
        }

        [Test]
        public void SendingCookies()
        {
            using (IRestClient client = restClientFactory.CreateRestClient())
            {
                Uri url = new Uri("http://httpbin.org/cookies");
                client.Get(url.ToString())
                    .AddCookie(new Cookie("green", "cookie", "/", url.Host))
                    .AddCookie(new Cookie("blue", "cookie2", "/", url.Host))
                    .Do();
                Assert.AreEqual((int)HttpStatusCode.OK, client.StatusCode);
                JObject responseJson = client.Response.AsJson();
                JObject cookiesJson = (JObject)responseJson["cookies"];
                Assert.AreEqual(2, cookiesJson.Count);
                Assert.AreEqual("cookie", cookiesJson["green"].Value<string>());
                Assert.AreEqual("cookie2", cookiesJson["blue"].Value<string>());
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