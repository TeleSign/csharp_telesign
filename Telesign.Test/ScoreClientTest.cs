using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using MockHttpServer;

namespace Telesign.Test;

[TestFixture]
[ExcludeFromCodeCoverage]
public class ScoreClientTest : IDisposable
{
    private string customerId = string.Empty;
    private string apiKey = string.Empty;
    private MockServer mockServer;
    private List<HttpListenerRequest> requests = [];
    private List<string> requestBodies = [];
    private List<Dictionary<string, string>> requestHeaders = [];
    private Func<HttpListenerRequest, HttpListenerResponse, Dictionary<string, string>, string>? handlerLambda;

    bool disposed = false;

    [SetUp]
    public void SetUp()
    {
        customerId = Environment.GetEnvironmentVariable("CUSTOMER_ID")?? "FFFFFFFF-EEEE-DDDD-1234-AB1234567890";
        apiKey = Environment.GetEnvironmentVariable("API_KEY") ?? "ABC12345yusumoN6BYsBVkh+yRJ5czgsnCehZaOYldPJdmFh6NeX8kunZ2zU1YWaUw/0wV6xfw==";

        requests = [];
        requestBodies = [];
        requestHeaders = [];

        handlerLambda = (req, rsp, prm) =>
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
        };

        mockServer = new MockServer(0, "/intelligence/phone", handlerLambda);
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
    public void TestScoreClientConstructors()
    {
        var ScoreClient = new ScoreClient(customerId, apiKey);
    }

    [Test]
public void TestScoreClientScore()
{
    ScoreClient client = new(customerId, apiKey, string.Format("http://localhost:{0}", mockServer.Port), "csharp_telesign", null, null);

    var optionalParams = new Dictionary<string, string>
    {
        { "account_id", "acct_123" },
        { "device_id", "device_456" },
        { "email_address", "user@example.com" },
        { "external_id", "ext_789" },
        { "originating_ip", "192.168.1.1" }
    };

    client.Score(
        phoneNumber: "15555555555",
        accountLifecycleEvent: "create",
        scoreParams: optionalParams
    );

    Assert.That(requests.Last().HttpMethod, Is.EqualTo("POST"), "method is not as expected");
    Assert.That(requests.Last().RawUrl, Is.EqualTo("/intelligence/phone"), "path is not as expected");
    Assert.That(requestHeaders.Last()["Content-Type"], Is.EqualTo("application/x-www-form-urlencoded"));
    Assert.That(requestHeaders.Last()["x-ts-auth-method"], Is.EqualTo("HMAC-SHA256"), "x-ts-auth-method header is not as expected");
    Assert.That(Guid.TryParse(requestHeaders.Last()["x-ts-nonce"], out _), Is.True, "x-ts-nonce header is not a valid UUID");
    Assert.That(DateTime.TryParse(requestHeaders.Last()["Date"], out _), Is.True, "Date header is not valid rfc2616 format");
    Assert.That(requestHeaders.Last()["Authorization"], Is.Not.Null);

    string body = requestBodies.Last();

    Assert.That(body.Contains("phone_number=15555555555"), Is.True, "phone_number not found in body");
    Assert.That(body.Contains("account_lifecycle_event=create"), Is.True, "account_lifecycle_event not found in body");
    Assert.That(body.Contains("account_id=acct_123"), Is.True, "account_id not found in body");
    Assert.That(body.Contains("device_id=device_456"), Is.True, "device_id not found in body");
    Assert.That(body.Contains("email_address=user%40example.com"), Is.True, "email_address not encoded correctly or missing");
    Assert.That(body.Contains("external_id=ext_789"), Is.True, "external_id not found in body");
    Assert.That(body.Contains("originating_ip=192.168.1.1"), Is.True, "originating_ip not found in body");
}


    [Test]
public async Task TestScoreClientStatusAsync()
{
    ScoreClient client = new(customerId, apiKey, string.Format("http://localhost:{0}", mockServer.Port), "csharp_telesign", null, null);

    var optionalParams = new Dictionary<string, string>
    {
        {"account_id", "acct_111"},
        {"device_id", "dev_222"},
        {"email_address", "async@example.com"},
        {"external_id", "ext_333"},
        {"originating_ip", "10.0.0.1"}
    };

    await client.ScoreAsync(
        phoneNumber: "15555555555",
        accountLifecycleEvent: "update",
        scoreParams: optionalParams
    );

    Assert.That(requests.Last().HttpMethod, Is.EqualTo("POST"), "method is not as expected");
    Assert.That(requests.Last().RawUrl, Is.EqualTo("/intelligence/phone"), "path is not as expected");
    Assert.That(requestHeaders.Last()["Content-Type"], Is.EqualTo("application/x-www-form-urlencoded"));
    Assert.That(requestHeaders.Last()["x-ts-auth-method"], Is.EqualTo("HMAC-SHA256"), "x-ts-auth-method header is not as expected");
    Assert.That(Guid.TryParse(requestHeaders.Last()["x-ts-nonce"], out _), Is.True, "x-ts-nonce header is not a valid UUID");
    Assert.That(DateTime.TryParse(requestHeaders.Last()["Date"], out _), Is.True, "Date header is not valid rfc2616 format");
    Assert.That(requestHeaders.Last()["Authorization"], Is.Not.Null);

    string body = requestBodies.Last();

    Assert.That(body.Contains("phone_number=15555555555"), Is.True, "phone_number not found in body");
    Assert.That(body.Contains("account_lifecycle_event=update"), Is.True, "account_lifecycle_event not found in body");
    Assert.That(body.Contains("account_id=acct_111"), Is.True, "account_id not found in body");
    Assert.That(body.Contains("device_id=dev_222"), Is.True, "device_id not found in body");
    Assert.That(body.Contains("email_address=async%40example.com"), Is.True, "email_address not encoded correctly or missing");
    Assert.That(body.Contains("external_id=ext_333"), Is.True, "external_id not found in body");
    Assert.That(body.Contains("originating_ip=10.0.0.1"), Is.True, "originating_ip not found in body");
}

[Test]
public void TestScoreClientScore_MissingPhoneNumber_ThrowsException()
{
    ScoreClient client = new(customerId, apiKey, $"http://localhost:{mockServer.Port}", "csharp_telesign", null, null);

    var ex = Assert.Throws<ArgumentException>(() =>
        client.Score(
            phoneNumber: "",
            accountLifecycleEvent: "create"
        )
    );

    Assert.That(ex.Message, Does.Contain("phoneNumber"));
}

[Test]
public void TestScoreClientScore_MissingEvent_ThrowsException()
{
    ScoreClient client = new(customerId, apiKey, $"http://localhost:{mockServer.Port}", "csharp_telesign", null, null);

    var ex = Assert.Throws<ArgumentException>(() =>
        client.Score(
            phoneNumber: "15555555555",
            accountLifecycleEvent: ""
        )
    );

    Assert.That(ex.Message, Does.Contain("accountLifecycleEvent"), "Expected accountLifecycleEvent validation error");
}

    [Test]
    public async Task TestScoreClientScoreAsync_MissingEvent_ThrowsException()
    {
        ScoreClient client = new(customerId, apiKey, $"http://localhost:{mockServer.Port}", "csharp_telesign", null, null);

        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await client.ScoreAsync(
                phoneNumber: "15555555555",
                accountLifecycleEvent: ""
            )
        );

        Assert.That(ex.Message, Does.Contain("accountLifecycleEvent"));
    }
}