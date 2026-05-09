using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyCosts.Application.Common;
using MyCosts.Application.Features.Auth.RegisterUser;
using MyCosts.Application.Features.ProductCategories.CreateProductCategory;
using MyCosts.Application.Features.Products.CreateProduct;
using MyCosts.Application.Features.Products.DeleteProduct;
using MyCosts.Domain.Users;
using MyCosts.IntegrationTests.Infrastructure;
using Xunit;

namespace MyCosts.IntegrationTests.Features.Products;

[Collection("Database")]
public class DeleteProductHandlerTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    private readonly IPasswordHasher<User> _hasher = new PasswordHasher<User>();

    private async Task<Guid> SeedUserAsync(string email)
    {
        var handler = new RegisterUserHandler(DbContext, _hasher);
        return (await handler.HandleAsync(new RegisterUserCommand(email, "Password1!"), CancellationToken.None)).Value;
    }

    private async Task<Guid> SeedCategoryAsync(Guid userId, string name)
    {
        var handler = new CreateProductCategoryHandler(DbContext);
        var cmd = new CreateProductCategoryCommand(userId, name);
        return (await handler.HandleAsync(cmd, CancellationToken.None)).Value;
    }

    private async Task<Guid> SeedProductAsync(Guid userId, Guid categoryId, string name)
    {
        var handler = new CreateProductHandler(DbContext);
        var cmd = new CreateProductCommand(userId, categoryId, name);
        return (await handler.HandleAsync(cmd, CancellationToken.None)).Value;
    }

    [Fact]
    public async Task HandleAsync_WithExistingProduct_DeletesIt()
    {
        var userId = await SeedUserAsync("user@example.com");
        var categoryId = await SeedCategoryAsync(userId, "Dairy");
        var productId = await SeedProductAsync(userId, categoryId, "Milk");
        var handler = new DeleteProductHandler(DbContext);
        var cmd = new DeleteProductCommand(userId, productId);

        var result = await handler.HandleAsync(cmd, CancellationToken.None);

        var exists = await DbContext.Products.AnyAsync(p => p.Id == productId);
        Assert.True(result.IsSuccess);
        Assert.False(exists);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentId_ReturnsNotFound()
    {
        var handler = new DeleteProductHandler(DbContext);
        var cmd = new DeleteProductCommand(Guid.CreateVersion7(), Guid.CreateVersion7());

        var result = await handler.HandleAsync(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_WithOtherUserProduct_ReturnsNotFound()
    {
        var ownerUserId = await SeedUserAsync("owner@example.com");
        var attackerUserId = await SeedUserAsync("attacker@example.com");
        var categoryId = await SeedCategoryAsync(ownerUserId, "Dairy");
        var productId = await SeedProductAsync(ownerUserId, categoryId, "Milk");
        var handler = new DeleteProductHandler(DbContext);
        var cmd = new DeleteProductCommand(attackerUserId, productId);

        var result = await handler.HandleAsync(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }
}
