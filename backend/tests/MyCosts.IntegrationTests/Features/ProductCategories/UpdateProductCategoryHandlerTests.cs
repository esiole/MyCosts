using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyCosts.Application.Common;
using MyCosts.Application.Features.Auth.RegisterUser;
using MyCosts.Application.Features.ProductCategories.CreateProductCategory;
using MyCosts.Application.Features.ProductCategories.UpdateProductCategory;
using MyCosts.Domain.Users;
using MyCosts.IntegrationTests.Infrastructure;
using Xunit;

namespace MyCosts.IntegrationTests.Features.ProductCategories;

[Collection("Database")]
public class UpdateProductCategoryHandlerTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    private readonly IPasswordHasher<User> _hasher = new PasswordHasher<User>();

    [Fact]
    public async Task HandleAsync_WithValidData_UpdatesCategory()
    {
        var registerHandler = new RegisterUserHandler(DbContext, _hasher);
        var registerCmd = new RegisterUserCommand("update-user@example.com", "Password1!");
        var userId = (await registerHandler.HandleAsync(registerCmd, CancellationToken.None)).Value;
        var createHandler = new CreateProductCategoryHandler(DbContext);
        var createCategoryCmd = new CreateProductCategoryCommand(userId, "OldName");
        var createResult = await createHandler.HandleAsync(createCategoryCmd, CancellationToken.None);
        var handler = new UpdateProductCategoryHandler(DbContext);
        var cmd = new UpdateProductCategoryCommand(userId, createResult.Value, "NewName");

        var result = await handler.HandleAsync(cmd, CancellationToken.None);

        var category = await DbContext.ProductCategories
            .AsNoTracking()
            .SingleOrDefaultAsync(c => c.Id == createResult.Value);
        Assert.True(result.IsSuccess);
        Assert.NotNull(category);
        Assert.Equal("NewName", category.Name);
    }

    [Fact]
    public async Task HandleAsync_WithDuplicateName_ReturnsConflict()
    {
        var registerHandler = new RegisterUserHandler(DbContext, _hasher);
        var registerCmd = new RegisterUserCommand("update-user@example.com", "Password1!");
        var userId = (await registerHandler.HandleAsync(registerCmd, CancellationToken.None)).Value;
        var createHandler = new CreateProductCategoryHandler(DbContext);
        var createCategoryCmd = new CreateProductCategoryCommand(userId, "FirstName");
        var first = await createHandler.HandleAsync(createCategoryCmd, CancellationToken.None);
        await createHandler.HandleAsync(new CreateProductCategoryCommand(userId, "Second"), CancellationToken.None);
        var handler = new UpdateProductCategoryHandler(DbContext);
        var cmd = new UpdateProductCategoryCommand(userId, first.Value, "Second");

        var result = await handler.HandleAsync(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorKind.Conflict, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentCategory_ReturnsNotFound()
    {
        var handler = new UpdateProductCategoryHandler(DbContext);
        var cmd = new UpdateProductCategoryCommand(Guid.CreateVersion7(), Guid.CreateVersion7(), "AnyName");
        var result = await handler.HandleAsync(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_WithOtherUserCategory_ReturnsNotFound()
    {
        var registerHandler = new RegisterUserHandler(DbContext, _hasher);
        var registerUserCmd = new RegisterUserCommand("update-user@example.com", "Password1!");
        var userId = (await registerHandler.HandleAsync(registerUserCmd, CancellationToken.None)).Value;
        var registerOtherUserCmd = new RegisterUserCommand("other-update@example.com", "Password1!");
        var otherUserId = (await registerHandler.HandleAsync(registerOtherUserCmd, CancellationToken.None)).Value;
        var createHandler = new CreateProductCategoryHandler(DbContext);
        var createCategoryCmd = new CreateProductCategoryCommand(otherUserId, "OtherCategory");
        var created = await createHandler.HandleAsync(createCategoryCmd, CancellationToken.None);
        var handler = new UpdateProductCategoryHandler(DbContext);
        var cmd = new UpdateProductCategoryCommand(userId, created.Value, "HijackedName");

        var result = await handler.HandleAsync(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public async Task HandleAsync_WithEmptyName_ReturnsValidation(string? invalidName)
    {
        var handler = new UpdateProductCategoryHandler(DbContext);
        var cmd = new UpdateProductCategoryCommand(Guid.CreateVersion7(), Guid.CreateVersion7(), invalidName!);

        var result = await handler.HandleAsync(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorKind.Validation, result.Error.Kind);
    }
}
