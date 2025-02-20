using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using MockHttpServer;

namespace Telesign.Test;

[TestFixture]
[ExcludeFromCodeCoverage]
public class PhoneIdClientTest : IDisposable
{
    private string customerId = string.Empty;
    private string apiKey = string.Empty;
    private MockServer mockServer;
    private List<HttpListenerRequest> requests = [];
    private List<string> requestBodies = [];
    private List<Dictionary<string, string>> requestHeaders = [];

    bool disposed = false;

    [SetUp]
    public void SetUp()
    {
        customerId = Environment.GetEnvironmentVariable("CUSTOMER_ID")?? "FFFFFFFF-EEEE-DDDD-1234-AB1234567890";
        apiKey = Environment.GetEnvironmentVariable("API_KEY") ?? "Example/idksdjKJD+==";

        requests = [];
        requestBodies = [];
        requestHeaders = [];

        mockServer = new MockServer(0, "/v1/phoneid/15555555555", (req, rsp, prm) =>
        {
            requests.Add(req);
            requestBodies.Add(req.Content());

            Dictionary<string, string> headers = new()
            {
                    {"Content-Type", req.Headers["Content-Type"] ?? string.Empty},
                    {"x-ts-auth-method", req.Headers["x-ts-auth-method"] ?? string.Empty},
                    {"x-ts-nonce", req.Headers["x-ts-nonce"] ?? string.Empty},
                    {"Date", req.Headers["Date"] ?? string.Empty},
                    {"Authorization", req.Headers["Authorization"] ?? string.Empty}
            };
            requestHeaders.Add(headers);

            return "{}";
        });
    }

    [TearDown]
    public void TearDown()
    {
        Dispose();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposed)
            return;

        mockServer.Dispose();
        disposed = true;
    }

    [Test]
    public void TestPhoneIdClientConstructors()
    {
        _ = new PhoneIdClient(customerId, apiKey);
    }


    [Test]
    public async Task TestPhoneIdClientPhoneIdAsync()
    {
        PhoneIdClient client = new(customerId, apiKey, string.Format("http://localhost:{0}", mockServer.Port));

        await client.PhoneIdAsync("15555555555");

        Assert.That(requests.Last().HttpMethod, Is.EqualTo("POST"), "method is not as expected");
        Assert.That(requests.Last().RawUrl, Is.EqualTo("/v1/phoneid/15555555555"), "path is not as expected");
        Assert.That(requestHeaders.Last()["Content-Type"], Is.EqualTo("application/json"), "Content-Type header is not as expected");
        Assert.That(requestHeaders.Last()["x-ts-auth-method"], Is.EqualTo("HMAC-SHA256"), "x-ts-auth-method header is not as expected");
        Assert.That(Guid.TryParse(requestHeaders.Last()["x-ts-nonce"], out Guid dummyGuid), Is.True, "x-ts-nonce header is not a valid UUID");
        Assert.That(DateTime.TryParse(requestHeaders.Last()["Date"], out DateTime dummyDateTime), Is.True, "Date header is not valid rfc2616 format");
        Assert.That(requestHeaders.Last()["Authorization"], Is.Not.Null);
    }
}
