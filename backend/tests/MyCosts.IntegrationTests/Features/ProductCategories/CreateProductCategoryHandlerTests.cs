using Microsoft.AspNetCore.Identity;
using MyCosts.Application.Common;
using MyCosts.Application.Features.Auth.RegisterUser;
using MyCosts.Application.Features.ProductCategories.CreateProductCategory;
using MyCosts.Domain.Users;
using MyCosts.IntegrationTests.Infrastructure;
using Xunit;

namespace MyCosts.IntegrationTests.Features.ProductCategories;

[Collection("Database")]
public class CreateProductCategoryHandlerTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    private readonly IPasswordHasher<User> _hasher = new PasswordHasher<User>();

    [Fact]
    public async Task HandleAsync_WithValidName_CreatesCategory()
    {
        var registerHandler = new RegisterUserHandler(DbContext, _hasher);
        var registerCmd = new RegisterUserCommand("user@example.com", "Password1!");
        var registerResult = await registerHandler.HandleAsync(registerCmd, CancellationToken.None);
        var userId = registerResult.Value;
        var handler = new CreateProductCategoryHandler(DbContext);
        var cmd = new CreateProductCategoryCommand(userId, "Dairy");

        var result = await handler.HandleAsync(cmd, CancellationToken.None);

        var category = await DbContext.ProductCategories.FindAsync(result.Value);
        Assert.True(result.IsSuccess);
        Assert.NotNull(category);
        Assert.Equal("Dairy", category.Name);
        Assert.Equal(userId, category.UserId);
    }

    [Fact]
    public async Task HandleAsync_WithDuplicateName_ReturnsConflict()
    {
        var registerHandler = new RegisterUserHandler(DbContext, _hasher);
        var registerCmd = new RegisterUserCommand("user@example.com", "Password1!");
        var registerResult = await registerHandler.HandleAsync(registerCmd, CancellationToken.None);
        var userId = registerResult.Value;
        var handler = new CreateProductCategoryHandler(DbContext);
        await handler.HandleAsync(new CreateProductCategoryCommand(userId, "Dairy"), CancellationToken.None);

        var result =
            await handler.HandleAsync(new CreateProductCategoryCommand(userId, "Dairy"), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorKind.Conflict, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_WithSameNameDifferentUser_Succeeds()
    {
        var registerHandler = new RegisterUserHandler(DbContext, _hasher);
        var registerUserACmd = new RegisterUserCommand("usera@example.com", "Password1!");
        var userAId = (await registerHandler.HandleAsync(registerUserACmd, CancellationToken.None)).Value;
        var registerUserBCmd = new RegisterUserCommand("userb@example.com", "Password1!");
        var userBId = (await registerHandler.HandleAsync(registerUserBCmd, CancellationToken.None)).Value;
        var handler = new CreateProductCategoryHandler(DbContext);
        await handler.HandleAsync(new CreateProductCategoryCommand(userAId, "Dairy"), CancellationToken.None);

        var result =
            await handler.HandleAsync(new CreateProductCategoryCommand(userBId, "Dairy"), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public async Task HandleAsync_WithEmptyName_ReturnsValidation(string? invalidName)
    {
        var handler = new CreateProductCategoryHandler(DbContext);
        var cmd = new CreateProductCategoryCommand(Guid.CreateVersion7(), invalidName!);

        var result = await handler.HandleAsync(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorKind.Validation, result.Error.Kind);
    }
}
