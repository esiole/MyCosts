using Microsoft.AspNetCore.Identity;
using MyCosts.Application.Common;
using MyCosts.Application.Features.Auth.RegisterUser;
using MyCosts.Domain.Users;
using MyCosts.IntegrationTests.Infrastructure;
using Xunit;

namespace MyCosts.IntegrationTests.Features.Auth;

[Collection("Database")]
public class RegisterUserHandlerTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    private readonly IPasswordHasher<User> _hasher = new PasswordHasher<User>();

    [Fact]
    public async Task HandleAsync_WithNewEmail_CreatesUser()
    {
        var handler = new RegisterUserHandler(DbContext, _hasher);
        var cmd = new RegisterUserCommand("new@example.com", "Password123!");

        var result = await handler.HandleAsync(cmd, CancellationToken.None);

        var user = await DbContext.Users.FindAsync(result.Value);
        Assert.True(result.IsSuccess);
        Assert.NotNull(user);
        Assert.Equal("new@example.com", user.Email);
    }

    [Fact]
    public async Task HandleAsync_WithNewEmail_HashesPassword()
    {
        var handler = new RegisterUserHandler(DbContext, _hasher);
        var cmd = new RegisterUserCommand("hash@example.com", "Password123!");

        var result = await handler.HandleAsync(cmd, CancellationToken.None);

        var user = await DbContext.Users.FindAsync(result.Value);
        Assert.True(result.IsSuccess);
        Assert.NotNull(user);
        Assert.NotEqual("Password123!", user.PasswordHash);
        Assert.NotEmpty(user.PasswordHash);
    }

    [Fact]
    public async Task HandleAsync_WithDuplicateEmail_ReturnsConflictError()
    {
        var handler = new RegisterUserHandler(DbContext, _hasher);
        var cmd = new RegisterUserCommand("dup@example.com", "Password123!");
        await handler.HandleAsync(cmd, CancellationToken.None);

        var result = await handler.HandleAsync(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorKind.Conflict, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_WithDuplicateEmailDifferentCase_ReturnsConflictError()
    {
        var handler = new RegisterUserHandler(DbContext, _hasher);
        var firstCmd = new RegisterUserCommand("case@example.com", "Password123!");
        await handler.HandleAsync(firstCmd, CancellationToken.None);
        var secondCmd = new RegisterUserCommand("CASE@EXAMPLE.COM", "Password123!");

        var result = await handler.HandleAsync(secondCmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorKind.Conflict, result.Error.Kind);
    }

    [Theory]
    [InlineData("  ")]
    [InlineData("notanemail")]
    public async Task HandleAsync_WithInvalidEmail_ReturnsValidationError(string invalidEmail)
    {
        var handler = new RegisterUserHandler(DbContext, _hasher);
        var cmd = new RegisterUserCommand(invalidEmail, "Password123!");

        var result = await handler.HandleAsync(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorKind.Validation, result.Error.Kind);
    }

    [Theory]
    [InlineData("abc")]
    public async Task HandleAsync_WithInvalidPassword_ReturnsValidationError(string invalidPassword)
    {
        var handler = new RegisterUserHandler(DbContext, _hasher);
        var cmd = new RegisterUserCommand("valid@example.com", invalidPassword);

        var result = await handler.HandleAsync(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorKind.Validation, result.Error.Kind);
    }
}
