using NUnit.Framework;
using System.Collections.Generic;
using MockHttpServer;
using System.Net;
using System.Linq;
using System;

namespace Telesign.Test
{
    [TestFixture]
    public class RestClientTest
    {
        private string customerId;
        private string apiKey;

        private MockServer mockServer;

        private List<HttpListenerRequest> requests;
        private List<string> requestBodies;
        private List<Dictionary<string, string>> requestHeaders;

        [SetUp]
        public void SetUp()
        {
            this.customerId = "FFFFFFFF-EEEE-DDDD-1234-AB1234567890";
            this.apiKey = "EXAMPLETE8sTgg45yusumoN6BYsBVkh+yRJ5czgsnCehZaOYldPJdmFh6NeX8kunZ2zU1YWaUw/0wV6xfw==";

            this.requests = new List<HttpListenerRequest>();
            this.requestBodies = new List<string>();
            this.requestHeaders = new List<Dictionary<string, string>>();

            this.mockServer = new MockServer(0, "/test/resource", (req, rsp, prm) =>
            {
                requests.Add(req);
                requestBodies.Add(req.Content());

                Dictionary<string, string> headers = new Dictionary<string, string>
                {
                    {"Content-Type", req.Headers["Content-Type"]},
                    {"x-ts-auth-method", req.Headers["x-ts-auth-method"]},
                    {"x-ts-nonce", req.Headers["x-ts-nonce"]},
                    {"Date", req.Headers["Date"]},
                    {"Authorization", req.Headers["Authorization"]}
                };
                requestHeaders.Add(headers);

                return "{}";
            });
        }

        [Test]
        public void TestRestClientConstructors()
        {
            RestClient client = new RestClient(this.customerId, this.apiKey);
        }

        [Test]
        public void TestGenerateTelesignHeadersWithPost()
        {
            string methodName = "POST";
            string dateRfc2616 = "Wed, 14 Dec 2016 18:20:12 GMT";
            string nonce = "A1592C6F-E384-4CDB-BC42-C3AB970369E9";
            string resource = "/v1/resource";
            string bodyParamsUrlEncoded = "test=param";

            string expectedAuthorizationHeader = "TSA FFFFFFFF-EEEE-DDDD-1234-AB1234567890:" +
                "2xVlmbrxLjYrrPun3G3WMNG6Jon4yKcTeOoK9DjXJ/Q=";

            Dictionary<string, string> actualHeaders = RestClient.GenerateTelesignHeaders(this.customerId,
                this.apiKey,
                methodName,
                resource,
                bodyParamsUrlEncoded,
                dateRfc2616,
                nonce,
                "unitTest");

            string actualAuthorizationHeader;
            actualHeaders.TryGetValue("Authorization", out actualAuthorizationHeader);

            Assert.AreEqual(expectedAuthorizationHeader, actualAuthorizationHeader, "Authorization header is not as expected");
        }

        [Test]
        public void TestGenerateTelesignHeadersUnicodeContent()
        {
            string methodName = "POST";
            string dateRfc2616 = "Wed, 14 Dec 2016 18:20:12 GMT";
            string nonce = "A1592C6F-E384-4CDB-BC42-C3AB970369E9";
            string resource = "/v1/resource";
            string bodyParamsUrlEncoded = "test=%CF%BF";

            string expectedAuthorizationHeader = "TSA FFFFFFFF-EEEE-DDDD-1234-AB1234567890:" +
                "h8d4I0RTxErbxYXuzCOtNqb/f0w3Ck8e5SEkGNj01+8=";

            Dictionary<string, string> actualHeaders = RestClient.GenerateTelesignHeaders(this.customerId,
                this.apiKey,
                methodName,
                resource,
                bodyParamsUrlEncoded,
                dateRfc2616,
                nonce,
                "unitTest");

            string actualAuthorizationHeader;
            actualHeaders.TryGetValue("Authorization", out actualAuthorizationHeader);

            Assert.AreEqual(expectedAuthorizationHeader, actualAuthorizationHeader, "Authorization header is not as expected");
        }

        [Test]
        public void TestGenerateTelesignHeadersWithGet()
        {

            string methodName = "GET";
            string dateRfc2616 = "Wed, 14 Dec 2016 18:20:12 GMT";
            string nonce = "A1592C6F-E384-4CDB-BC42-C3AB970369E9";
            string resource = "/v1/resource";

            string expectedAuthorizationHeader = "TSA FFFFFFFF-EEEE-DDDD-1234-AB1234567890:" +
                "aUm7I+9GKl3ww7PNeeJntCT0iS7b+EmRKEE4LnRzChQ=";

            Dictionary<string, string> actualHeaders = RestClient.GenerateTelesignHeaders(this.customerId,
                this.apiKey,
                methodName,
                resource,
                "",
                dateRfc2616,
                nonce,
                "unitTest");

            string actualAuthorizationHeader;
            actualHeaders.TryGetValue("Authorization", out actualAuthorizationHeader);

            Assert.AreEqual(expectedAuthorizationHeader, actualAuthorizationHeader, "Authorization header is not as expected");
        }

        [Test]
        public void TestGenerateTelesignHeadersDefaultValues()
        {
            string methodName = "GET";
            string resource = "/v1/resource";

            Dictionary<string, string> actualHeaders = RestClient.GenerateTelesignHeaders(this.customerId,
                this.apiKey,
                methodName,
                resource,
                "",
                null,
                null,
                null);

            Guid dummyGuid;
            Assert.IsTrue(Guid.TryParse(actualHeaders["x-ts-nonce"], out dummyGuid), "x-ts-nonce header is not a valid UUID");

            DateTime dummyDateTime;
            Assert.IsTrue(DateTime.TryParse(actualHeaders["Date"], out dummyDateTime), "Date header is not valid rfc2616 format");
        }

        [Test]
        public void TestRestClientPost()
        {
            string testResource = "/test/resource";
            Dictionary<string, string> testParams = new Dictionary<string, string>();
            testParams.Add("test", "123_\u03ff_test");

            RestClient client = new RestClient(this.customerId,
                                               this.apiKey,
                                               string.Format("http://localhost:{0}", this.mockServer.Port));

            client.Post(testResource, testParams);

            Assert.AreEqual("POST", this.requests.Last().HttpMethod, "method is not as expected");
            Assert.AreEqual("/test/resource", this.requests.Last().RawUrl, "path is not as expected");

            Assert.AreEqual("test=123_%CF%BF_test", this.requestBodies.Last(), "body is not as expected");

            Assert.AreEqual("application/x-www-form-urlencoded", this.requestHeaders.Last()["Content-Type"], "Content-Type header is not as expected");
            Assert.AreEqual("HMAC-SHA256", this.requestHeaders.Last()["x-ts-auth-method"], "x-ts-auth-method header is not as expected");

            Guid dummyGuid;
            Assert.IsTrue(Guid.TryParse(this.requestHeaders.Last()["x-ts-nonce"], out dummyGuid), "x-ts-nonce header is not a valid UUID");

            DateTime dummyDateTime;
            Assert.IsTrue(DateTime.TryParse(this.requestHeaders.Last()["Date"], out dummyDateTime), "Date header is not valid rfc2616 format");

            Assert.IsNotNull(this.requestHeaders.Last()["Authorization"]);
        }

        [Test]
        public void TestRestClientGet()
        {
            string testResource = "/test/resource";
            Dictionary<string, string> testParams = new Dictionary<string, string>();
            testParams.Add("test", "123_\u03ff_test");

            RestClient client = new RestClient(this.customerId,
                                               this.apiKey,
                                               string.Format("http://localhost:{0}", this.mockServer.Port));

            client.Get(testResource, testParams);

            Assert.AreEqual("GET", this.requests.Last().HttpMethod, "method is not as expected");
            Assert.AreEqual("/test/resource?test=123_%CF%BF_test", this.requests.Last().RawUrl, "path is not as expected");

            Assert.AreEqual("", this.requestBodies.Last(), "body is not as expected");

            Assert.AreEqual(null, this.requestHeaders.Last()["Content-Type"], "Content-Type header is not as expected");
            Assert.AreEqual("HMAC-SHA256", this.requestHeaders.Last()["x-ts-auth-method"], "x-ts-auth-method header is not as expected");

            Guid dummyGuid;
            Assert.IsTrue(Guid.TryParse(this.requestHeaders.Last()["x-ts-nonce"], out dummyGuid), "x-ts-nonce header is not a valid UUID");

            DateTime dummyDateTime;
            Assert.IsTrue(DateTime.TryParse(this.requestHeaders.Last()["Date"], out dummyDateTime), "Date header is not valid rfc2616 format");

            Assert.IsNotNull(this.requestHeaders.Last()["Authorization"]);
        }

        [Test]
        public void TestRestClientPut()
        {
            string testResource = "/test/resource";
            Dictionary<string, string> testParams = new Dictionary<string, string>();
            testParams.Add("test", "123_\u03ff_test");

            RestClient client = new RestClient(this.customerId,
                                               this.apiKey,
                                               string.Format("http://localhost:{0}", this.mockServer.Port));

            client.Put(testResource, testParams);

            Assert.AreEqual("PUT", this.requests.Last().HttpMethod, "method is not as expected");
            Assert.AreEqual("/test/resource", this.requests.Last().RawUrl, "path is not as expected");

            Assert.AreEqual("test=123_%CF%BF_test", this.requestBodies.Last(), "body is not as expected");

            Assert.AreEqual("application/x-www-form-urlencoded", this.requestHeaders.Last()["Content-Type"], "Content-Type header is not as expected");
            Assert.AreEqual("HMAC-SHA256", this.requestHeaders.Last()["x-ts-auth-method"], "x-ts-auth-method header is not as expected");

            Guid dummyGuid;
            Assert.IsTrue(Guid.TryParse(this.requestHeaders.Last()["x-ts-nonce"], out dummyGuid), "x-ts-nonce header is not a valid UUID");

            DateTime dummyDateTime;
            Assert.IsTrue(DateTime.TryParse(this.requestHeaders.Last()["Date"], out dummyDateTime), "Date header is not valid rfc2616 format");

            Assert.IsNotNull(this.requestHeaders.Last()["Authorization"]);
        }

        [Test]
        public void TestRestClientDelete()
        {
            string testResource = "/test/resource";
            Dictionary<string, string> testParams = new Dictionary<string, string>();
            testParams.Add("test", "123_\u03ff_test");

            RestClient client = new RestClient(this.customerId,
                                               this.apiKey,
                                               string.Format("http://localhost:{0}", this.mockServer.Port));

            client.Delete(testResource, testParams);

            Assert.AreEqual("DELETE", this.requests.Last().HttpMethod, "method is not as expected");
            Assert.AreEqual("/test/resource?test=123_%CF%BF_test", this.requests.Last().RawUrl, "path is not as expected");

            Assert.AreEqual("", this.requestBodies.Last(), "body is not as expected");

            Assert.AreEqual(null, this.requestHeaders.Last()["Content-Type"], "Content-Type header is not as expected");
            Assert.AreEqual("HMAC-SHA256", this.requestHeaders.Last()["x-ts-auth-method"], "x-ts-auth-method header is not as expected");

            Guid dummyGuid;
            Assert.IsTrue(Guid.TryParse(this.requestHeaders.Last()["x-ts-nonce"], out dummyGuid), "x-ts-nonce header is not a valid UUID");

            DateTime dummyDateTime;
            Assert.IsTrue(DateTime.TryParse(this.requestHeaders.Last()["Date"], out dummyDateTime), "Date header is not valid rfc2616 format");

            Assert.IsNotNull(this.requestHeaders.Last()["Authorization"]);
        }
    }
}
