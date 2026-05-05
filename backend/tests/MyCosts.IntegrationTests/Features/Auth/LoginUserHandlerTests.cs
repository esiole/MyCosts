using Microsoft.AspNetCore.Identity;
using MyCosts.Application.Common;
using MyCosts.Application.Features.Auth.LoginUser;
using MyCosts.Application.Features.Auth.RegisterUser;
using MyCosts.Domain.Users;
using MyCosts.IntegrationTests.Infrastructure;
using Xunit;

namespace MyCosts.IntegrationTests.Features.Auth;

[Collection("Database")]
public class LoginUserHandlerTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    private readonly IPasswordHasher<User> _hasher = new PasswordHasher<User>();

    [Fact]
    public async Task HandleAsync_WithValidCredentials_ReturnsUser()
    {
        var registerHandler = new RegisterUserHandler(DbContext, _hasher);
        var registerCmd = new RegisterUserCommand("valid@example.com", "Password123!");
        await registerHandler.HandleAsync(registerCmd, CancellationToken.None);
        var loginHandler = new LoginUserHandler(DbContext, _hasher);
        var loginCmd = new LoginUserCommand("valid@example.com", "Password123!");

        var result = await loginHandler.HandleAsync(loginCmd, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("valid@example.com", result.Value.Email);
    }

    [Fact]
    public async Task HandleAsync_WithWrongPassword_ReturnsUnauthorizedError()
    {
        var registerHandler = new RegisterUserHandler(DbContext, _hasher);
        var registerCmd = new RegisterUserCommand("wrong@example.com", "Password123!");
        await registerHandler.HandleAsync(registerCmd, CancellationToken.None);
        var loginHandler = new LoginUserHandler(DbContext, _hasher);
        var loginCmd = new LoginUserCommand("wrong@example.com", "WrongPassword!");

        var result = await loginHandler.HandleAsync(loginCmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorKind.Unauthorized, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_WithUnknownEmail_ReturnsUnauthorizedError()
    {
        var login = new LoginUserHandler(DbContext, _hasher);
        var cmd = new LoginUserCommand("nobody@example.com", "Password123!");

        var result = await login.HandleAsync(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorKind.Unauthorized, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_WithUpperCaseEmail_ReturnsUser()
    {
        var registerHandler = new RegisterUserHandler(DbContext, _hasher);
        var registerCmd = new RegisterUserCommand("casetest@example.com", "Password123!");
        await registerHandler.HandleAsync(registerCmd, CancellationToken.None);
        var loginHandler = new LoginUserHandler(DbContext, _hasher);
        var loginCmd = new LoginUserCommand("CASETEST@EXAMPLE.COM", "Password123!");

        var result = await loginHandler.HandleAsync(loginCmd, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("casetest@example.com", result.Value.Email);
    }

    [Fact]
    public async Task HandleAsync_WithMalformedEmail_ReturnsUnauthorizedError()
    {
        var login = new LoginUserHandler(DbContext, _hasher);
        var cmd = new LoginUserCommand("notanemail", "Password123!");

        var result = await login.HandleAsync(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorKind.Unauthorized, result.Error.Kind);
    }
}
