using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using MyCosts.IntegrationTests.Infrastructure;
using Xunit;

namespace MyCosts.IntegrationTests.Features.Auth;

[Collection("App")]
public class AuthEndpointsTests(AppFixture fixture)
{
    [Fact]
    public async Task Register_WithValidData_Returns200()
    {
        var client = fixture.CreateClient();
        var request = new { email = $"reg-{Guid.NewGuid()}@example.com", password = "Password123!" };

        var response = await client.PostAsJsonAsync("/api/v1/auth/register", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_Returns409()
    {
        var client = fixture.CreateClient();
        var request = new { email = $"dup-{Guid.NewGuid()}@example.com", password = "Password123!" };
        await client.PostAsJsonAsync("/api/v1/auth/register", request);

        var response = await client.PostAsJsonAsync("/api/v1/auth/register", request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithValidCredentials_SetsAuthCookie()
    {
        var client = fixture.CreateClient();
        var email = $"login-{Guid.NewGuid()}@example.com";
        var registerRequest = new { email, password = "Password123!" };
        await client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);
        var loginRequest = new { email, password = "Password123!" };

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.Contains("Set-Cookie"));
    }

    [Fact]
    public async Task Login_WithInvalidPassword_Returns401()
    {
        var client = fixture.CreateClient();
        var email = $"badpwd-{Guid.NewGuid()}@example.com";
        var registerRequest = new { email, password = "Password123!" };
        await client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);
        var loginRequest = new { email, password = "WrongPassword!" };

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Logout_WithCookieAuth_Returns200()
    {
        var client = fixture.CreateClient();
        var email = $"logout-{Guid.NewGuid()}@example.com";
        await client.PostAsJsonAsync("/api/v1/auth/register", new { email, password = "Password123!" });
        await client.PostAsJsonAsync("/api/v1/auth/login", new { email, password = "Password123!" });

        var response = await client.PostAsync("/api/v1/auth/logout", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Logout_WithoutAuth_Returns401()
    {
        var client = fixture.CreateClient();

        var response = await client.PostAsync("/api/v1/auth/logout", null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task MobileLogin_WithValidCredentials_ReturnsToken()
    {
        var client = fixture.CreateClient();
        var email = $"mob-{Guid.NewGuid()}@example.com";
        var registerRequest = new { email, password = "Password123!" };
        await client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);
        var loginRequest = new { email, password = "Password123!" };

        var response = await client.PostAsJsonAsync("/api/v1/auth/mobile/login", loginRequest);

        var body = await response.Content.ReadFromJsonAsync<TokenResponse>();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body?.Token);
        Assert.NotEmpty(body.Token);
    }

    [Fact]
    public async Task MobileLogin_WithInvalidPassword_Returns401()
    {
        var client = fixture.CreateClient();
        var email = $"mobbad-{Guid.NewGuid()}@example.com";
        var registerRequest = new { email, password = "Password123!" };
        await client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);
        var loginRequest = new { email, password = "WrongPassword!" };

        var response = await client.PostAsJsonAsync("/api/v1/auth/mobile/login", loginRequest);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task MobileLogout_WithBearerToken_Returns200()
    {
        var client = fixture.CreateClient();
        var email = $"moblout-{Guid.NewGuid()}@example.com";
        await client.PostAsJsonAsync("/api/v1/auth/register", new { email, password = "Password123!" });
        var loginRequest = new { email, password = "Password123!" };
        var loginResp = await client.PostAsJsonAsync("/api/v1/auth/mobile/login", loginRequest);
        var body = await loginResp.Content.ReadFromJsonAsync<TokenResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", body!.Token);

        var response = await client.PostAsync("/api/v1/auth/mobile/logout", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task MobileLogout_WithoutToken_Returns401()
    {
        var client = fixture.CreateClient();

        var response = await client.PostAsync("/api/v1/auth/mobile/logout", null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Logout_WithBearerToken_Returns401()
    {
        var client = fixture.CreateClient();
        var email = $"scheme-{Guid.NewGuid()}@example.com";
        await client.PostAsJsonAsync("/api/v1/auth/register", new { email, password = "Password123!" });
        var loginRequest = new { email, password = "Password123!" };
        var loginResp = await client.PostAsJsonAsync("/api/v1/auth/mobile/login", loginRequest);
        var body = await loginResp.Content.ReadFromJsonAsync<TokenResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", body!.Token);

        var response = await client.PostAsync("/api/v1/auth/logout", null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task MobileLogout_WithCookieAuth_Returns401()
    {
        var client = fixture.CreateClient();
        var email = $"scheme2-{Guid.NewGuid()}@example.com";
        await client.PostAsJsonAsync("/api/v1/auth/register", new { email, password = "Password123!" });
        await client.PostAsJsonAsync("/api/v1/auth/login", new { email, password = "Password123!" });

        var response = await client.PostAsync("/api/v1/auth/mobile/logout", null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Register_WithShortPassword_Returns400()
    {
        var client = fixture.CreateClient();
        var request = new { email = "test@example.com", password = "short" };

        var response = await client.PostAsJsonAsync("/api/v1/auth/register", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_WithMalformedEmail_Returns400()
    {
        var client = fixture.CreateClient();
        var request = new { email = "notanemail", password = "Password123!" };

        var response = await client.PostAsJsonAsync("/api/v1/auth/register", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}

file record TokenResponse(string Token);
