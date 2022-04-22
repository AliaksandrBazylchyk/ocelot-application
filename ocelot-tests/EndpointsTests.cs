using IdentityModel.Client;
using NUnit.Framework;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ocelot_common;

namespace ocelot_tests;

public class EndpointsTests
{
    private TokenResponse _token = new();

    [OneTimeSetUp]
    public async Task IdentitySetup()
    {
        var httpClient = new HttpClient();
        var disco = await httpClient.GetDiscoveryDocumentAsync(Constants.IdentityServerUrl);
        if (disco.IsError) throw new Exception(disco.Error);

        var credentials = new ClientCredentialsTokenRequest
        {
            Address = disco.TokenEndpoint,
            ClientId = "client",
            ClientSecret = "secret_key",
            Scope = "API"
        };
        var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(credentials);
        switch (tokenResponse.HttpStatusCode)
        {
            case HttpStatusCode.BadGateway:
                Assert.Fail("Identity server is down");
                break;
            case HttpStatusCode.Unauthorized:
                Assert.Fail(HttpStatusErrorDescription.Unauthorized);
                break;
            case HttpStatusCode.OK:
                break;
            default:
                Assert.Fail($"Unexpectedly error: {tokenResponse.ErrorDescription}");
                break;
        }

        _token = tokenResponse;

        httpClient.Dispose();

        Assert.Pass();
    }


    [Test(Description = "Secured endpoint without input data (GET method)")]
    public async Task ShouldReturn200WithDataSecured()
    {
        var httpClient = new HttpClient();

        var request = new HttpRequestMessage(HttpMethod.Get, Constants.SecuredApiGetRequest);
        request.Headers.Add(HttpRequestHeader.Authorization.ToString(), $"Bearer {_token.AccessToken}");

        var response = await httpClient.SendAsync(request);
        switch (response.StatusCode)
        {
            case HttpStatusCode.BadGateway:
                Assert.Fail("Secured API is down");
                break;
            case HttpStatusCode.Unauthorized:
                Assert.Fail(HttpStatusErrorDescription.Unauthorized);
                break;
            case HttpStatusCode.OK:
                break;
            default:
                Assert.Fail($"Unexpectedly error: {await response.Content.ReadAsStringAsync()}");
                break;
        }

        var answer = await response.Content.ReadAsStringAsync();

        httpClient.Dispose();

        Assert.IsNotNull(answer);
    }

    [Test(Description = "Secured endpoint with input data (GET method).")]
    [TestCase(12)]
    public async Task ShouldReturn200WithValidAnswerSecured(int number)
    {
        var httpClient = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, $"{Constants.SecuredApiGetRequest}/{number}");

        request.Headers.Add(HttpRequestHeader.Authorization.ToString(), $"Bearer {_token.AccessToken}");

        var response = await httpClient.SendAsync(request);
        switch (response.StatusCode)
        {
            case HttpStatusCode.BadGateway:
                Assert.Fail("Secured API is down");
                break;
            case HttpStatusCode.Unauthorized:
                Assert.Fail(HttpStatusErrorDescription.Unauthorized);
                break;
            case HttpStatusCode.OK:
                break;
            default:
                Assert.Fail($"Unexpectedly error: {await response.Content.ReadAsStringAsync()}");
                break;
        }

        var content = await response.Content.ReadAsStringAsync();
        var answer = int.Parse(content.Split(' ').Last());

        httpClient.Dispose();

        Assert.AreEqual(answer, number * number, $"Return 200 with data: '{answer}' but excepted {number * number}");
    }

    [Test(Description = "Secured endpoint with input data (POST method)")]
    [TestCase("Hello", "World")]
    public async Task ShouldReturn200WithValidAnswerSecured(string firstName, string lastName)
    {
        var httpClient = new HttpClient();

        var request = new HttpRequestMessage(HttpMethod.Post, Constants.SecuredApiPostRequest);
        request.Headers.Add(HttpRequestHeader.Authorization.ToString(), $"Bearer {_token.AccessToken}");
        request.Content = JsonContent.Create(new {FirstName = firstName, LastName = lastName});

        var response = await httpClient.SendAsync(request);
        switch (response.StatusCode)
        {
            case HttpStatusCode.BadGateway:
                Assert.Fail("Secured API is down");
                break;
            case HttpStatusCode.Unauthorized:
                Assert.Fail(HttpStatusErrorDescription.Unauthorized);
                break;
            case HttpStatusCode.UnsupportedMediaType:
                Assert.Fail(HttpStatusErrorDescription.UnsupportedMediaType);
                break;
            case HttpStatusCode.OK:
                break;
            default:
                Assert.Fail($"Unexpectedly error: {await response.Content.ReadAsStringAsync()}");
                break;
        }

        var answer = await response.Content.ReadAsStringAsync();

        httpClient.Dispose();

        Assert.AreEqual(
            answer,
            $"Your secured name: {firstName} {lastName}",
            $"Return 200 with data: \"{answer}\" but excepted \"Your secured name: {firstName} {lastName}\""
        );
    }

    [Test(Description = "Unsecured endpoint with input data (Get method)")]
    public async Task ShouldReturn200WithDataUnsecured()
    {
        var httpClient = new HttpClient();

        var request = new HttpRequestMessage(HttpMethod.Get, Constants.UnsecuredApiGetRequest);

        var response = await httpClient.SendAsync(request);
        switch (response.StatusCode)
        {
            case HttpStatusCode.BadGateway:
                Assert.Fail("Unsecured API is down");
                break;
            case HttpStatusCode.Unauthorized:
                Assert.Fail(HttpStatusErrorDescription.Unauthorized);
                break;
            case HttpStatusCode.OK:
                break;
            default:
                Assert.Fail($"Unexpectedly error: {await response.Content.ReadAsStringAsync()}");
                break;
        }

        var answer = await response.Content.ReadAsStringAsync();

        httpClient.Dispose();

        Assert.IsNotNull(answer);
    }
}