using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyCosts.Application.Common;
using MyCosts.Application.Features.Auth.RegisterUser;
using MyCosts.Application.Features.ProductCategories.CreateProductCategory;
using MyCosts.Application.Features.ProductCategories.DeleteProductCategory;
using MyCosts.Domain.Users;
using MyCosts.IntegrationTests.Infrastructure;
using Xunit;

namespace MyCosts.IntegrationTests.Features.ProductCategories;

[Collection("Database")]
public class DeleteProductCategoryHandlerTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    private readonly IPasswordHasher<User> _hasher = new PasswordHasher<User>();

    [Fact]
    public async Task HandleAsync_WithExistingCategory_DeletesIt()
    {
        var registerHandler = new RegisterUserHandler(DbContext, _hasher);
        var registerUserCmd = new RegisterUserCommand("delete-user@example.com", "Password1!");
        var userId = (await registerHandler.HandleAsync(registerUserCmd, CancellationToken.None)).Value;
        var createHandler = new CreateProductCategoryHandler(DbContext);
        var createCategoryCmd = new CreateProductCategoryCommand(userId, "ToDelete");
        var createResult = await createHandler.HandleAsync(createCategoryCmd, CancellationToken.None);
        var handler = new DeleteProductCategoryHandler(DbContext);
        var cmd = new DeleteProductCategoryCommand(userId, createResult.Value);

        var result = await handler.HandleAsync(cmd, CancellationToken.None);

        var exists = await DbContext.ProductCategories.AnyAsync(c => c.Id == createResult.Value);
        Assert.True(result.IsSuccess);
        Assert.False(exists);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentId_ReturnsNotFound()
    {
        var handler = new DeleteProductCategoryHandler(DbContext);
        var cmd = new DeleteProductCategoryCommand(Guid.CreateVersion7(), Guid.CreateVersion7());

        var result = await handler.HandleAsync(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_WithOtherUserCategory_ReturnsNotFound()
    {
        var registerHandler = new RegisterUserHandler(DbContext, _hasher);
        var registerUserCmd = new RegisterUserCommand("delete-user@example.com", "Password1!");
        var userId = (await registerHandler.HandleAsync(registerUserCmd, CancellationToken.None)).Value;
        var registerOtherUserCmd = new RegisterUserCommand("other-delete@example.com", "Password1!");
        var otherUserId = (await registerHandler.HandleAsync(registerOtherUserCmd, CancellationToken.None)).Value;
        var createHandler = new CreateProductCategoryHandler(DbContext);
        var createCategoryCmd = new CreateProductCategoryCommand(otherUserId, "OtherCategory");
        var created = await createHandler.HandleAsync(createCategoryCmd, CancellationToken.None);
        var handler = new DeleteProductCategoryHandler(DbContext);
        var cmd = new DeleteProductCategoryCommand(userId, created.Value);

        var result = await handler.HandleAsync(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }
}
