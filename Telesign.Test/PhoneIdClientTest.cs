using NUnit.Framework;
using System.Collections.Generic;
using MockHttpServer;
using System.Net;
using System.Linq;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Telesign.Test
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class PhoneIdClientTest : IDisposable
    {
        private string customerId;
        private string apiKey;

        private MockServer mockServer;

        private List<HttpListenerRequest> requests;
        private List<string> requestBodies;
        private List<Dictionary<string, string>> requestHeaders;

        bool disposed = false;

        [SetUp]
        public void SetUp()
        {
            this.customerId = "FFFFFFFF-EEEE-DDDD-1234-AB1234567890";
            this.apiKey = "EXAMPLETE8sTgg45yusumoN6BYsBVkh+yRJ5czgsnCehZaOYldPJdmFh6NeX8kunZ2zU1YWaUw/0wV6xfw==";

            this.requests = new List<HttpListenerRequest>();
            this.requestBodies = new List<string>();
            this.requestHeaders = new List<Dictionary<string, string>>();

            this.mockServer = new MockServer(0, "/v1/phoneid/15555555555", (req, rsp, prm) =>
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

        [TearDown]
        public void TearDown()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
                return;

            this.mockServer.Dispose();
            this.disposed = true;
        }

        [Test]
        public void TestPhoneIdClientConstructors()
        {
            var phoneIdClient = new PhoneIdClient(this.customerId, this.apiKey);
        }


        [Test]
        public async Task TestPhoneIdClientPhoneIdAsync()
        {

            var client = new PhoneIdClient(this.customerId,
                this.apiKey,
                string.Format("http://localhost:{0}", this.mockServer.Port));

            await client.PhoneIdAsync("15555555555");

            Assert.AreEqual("POST", this.requests.Last().HttpMethod, "method is not as expected");
            Assert.AreEqual("/v1/phoneid/15555555555", this.requests.Last().RawUrl, "path is not as expected");
            Assert.AreEqual("application/json", this.requestHeaders.Last()["Content-Type"],
                "Content-Type header is not as expected");
            Assert.AreEqual("HMAC-SHA256", this.requestHeaders.Last()["x-ts-auth-method"],
                "x-ts-auth-method header is not as expected");

            Guid dummyGuid;
            Assert.IsTrue(Guid.TryParse(this.requestHeaders.Last()["x-ts-nonce"], out dummyGuid),
                "x-ts-nonce header is not a valid UUID");

            DateTime dummyDateTime;
            Assert.IsTrue(DateTime.TryParse(this.requestHeaders.Last()["Date"], out dummyDateTime),
                "Date header is not valid rfc2616 format");

            Assert.IsNotNull(this.requestHeaders.Last()["Authorization"]);
        }
    }
}