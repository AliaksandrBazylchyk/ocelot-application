using IdentityModel.Client;
using NUnit.Framework;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;

namespace ocelot_tests;

public class EndpointsTests
{
    private TokenResponse _token = new();

    [OneTimeSetUp]
    public async Task IdentitySetup()
    {
        var client = new HttpClient();
        var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
        if (disco.IsError) throw new Exception(disco.Error);

        var credentials = new ClientCredentialsTokenRequest
        {
            Address = disco.TokenEndpoint,
            ClientId = "client",
            ClientSecret = "secret_key",
            Scope = "API"
        };
        var tokenResponse = await client.RequestClientCredentialsTokenAsync(credentials);
        switch (tokenResponse.HttpStatusCode)
        {
            case HttpStatusCode.BadGateway:
                Assert.Fail("Identity server is down");
                break;
            case HttpStatusCode.Unauthorized:
                Assert.Fail("Bad credentials");
                break;
            case HttpStatusCode.OK:
                break;
            default:
                Assert.Fail($"Unexpectedly error: {tokenResponse.ErrorDescription}");
                break;
        }

        client.Dispose();
        _token = tokenResponse;

        Assert.Pass();
    }


    [Test(Description = "Secured endpoint without input data (GET method)")]
    public async Task ShouldReturn200WithDataSecured()
    {
        var httpClient = new HttpClient();

        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:4000/secured");
        request.Headers.Add(HttpRequestHeader.Authorization.ToString(), $"Bearer {_token.AccessToken}");

        var response = await httpClient.SendAsync(request);
        switch (response.StatusCode)
        {
            case HttpStatusCode.BadGateway:
                Assert.Fail("Secured API is down");
                break;
            case HttpStatusCode.Unauthorized:
                Assert.Fail("Bad credentials");
                break;
            case HttpStatusCode.OK:
                break;
            default:
                Assert.Fail($"Unexpectedly error: {await response.Content.ReadAsStringAsync()}");
                break;
        }

        var answer = await response.Content.ReadAsStringAsync();

        Assert.IsNotNull(answer);
    }

    [Test(Description = "Secured endpoint with input data (GET method).")]
    [TestCase(12)]
    public async Task ShouldReturn200WithValidAnswerSecured(int number)
    {
        var httpClient = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, $"http://localhost:4000/secured/{number}");

        request.Headers.Add(HttpRequestHeader.Authorization.ToString(), $"Bearer {_token.AccessToken}");

        var response = await httpClient.SendAsync(request);
        switch (response.StatusCode)
        {
            case HttpStatusCode.BadGateway:
                Assert.Fail("Secured API is down");
                break;
            case HttpStatusCode.Unauthorized:
                Assert.Fail("Bad credentials");
                break;
            case HttpStatusCode.OK:
                break;
            default:
                Assert.Fail($"Unexpectedly error: {await response.Content.ReadAsStringAsync()}");
                break;
        }

        var content = await response.Content.ReadAsStringAsync();
        var answer = int.Parse(content.Split(' ').Last());

        Assert.AreEqual(answer, number * number, $"Return 200 with data: '{answer}' but excepted {number * number}");
    }

    [Test(Description = "Secured endpoint with input data (POST method)")]
    [TestCase("Hello", "World")]
    public async Task ShouldReturn200WithValidAnswerSecured(string firstName, string lastName)
    {
        var httpClient = new HttpClient();

        var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:4000/secured");
        request.Headers.Add(HttpRequestHeader.Authorization.ToString(), $"Bearer {_token.AccessToken}");
        request.Content = JsonContent.Create(new {FirstName = firstName, LastName = lastName});

        var response = await httpClient.SendAsync(request);
        switch (response.StatusCode)
        {
            case HttpStatusCode.BadGateway:
                Assert.Fail("Secured API is down");
                break;
            case HttpStatusCode.Unauthorized:
                Assert.Fail("Bad credentials");
                break;
            case HttpStatusCode.UnsupportedMediaType:
                Assert.Fail("Wrong body");
                break;
            case HttpStatusCode.OK:
                break;
            default:
                Assert.Fail($"Unexpectedly error: {await response.Content.ReadAsStringAsync()}");
                break;
        }

        var answer = await response.Content.ReadAsStringAsync();

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

        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:4000/WeatherForecast");

        var response = await httpClient.SendAsync(request);
        switch (response.StatusCode)
        {
            case HttpStatusCode.BadGateway:
                Assert.Fail("Unsecured API is down");
                break;
            case HttpStatusCode.Unauthorized:
                Assert.Fail("Bad credentials");
                break;
            case HttpStatusCode.OK:
                break;
            default:
                Assert.Fail($"Unexpectedly error: {await response.Content.ReadAsStringAsync()}");
                break;
        }

        var answer = await response.Content.ReadAsStringAsync();

        Assert.IsNotNull(answer);
    }
}