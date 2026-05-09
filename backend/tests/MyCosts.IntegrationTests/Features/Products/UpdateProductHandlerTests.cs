using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyCosts.Application.Common;
using MyCosts.Application.Features.Auth.RegisterUser;
using MyCosts.Application.Features.ProductCategories.CreateProductCategory;
using MyCosts.Application.Features.Products.CreateProduct;
using MyCosts.Application.Features.Products.UpdateProduct;
using MyCosts.Domain.Users;
using MyCosts.IntegrationTests.Infrastructure;
using Xunit;

namespace MyCosts.IntegrationTests.Features.Products;

[Collection("Database")]
public class UpdateProductHandlerTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
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
    public async Task HandleAsync_WithValidData_UpdatesProduct()
    {
        var userId = await SeedUserAsync("user@example.com");
        var categoryId = await SeedCategoryAsync(userId, "Dairy");
        var newCategoryId = await SeedCategoryAsync(userId, "Beverages");
        var productId = await SeedProductAsync(userId, categoryId, "Milk");
        var handler = new UpdateProductHandler(DbContext);
        var cmd = new UpdateProductCommand(userId, productId, newCategoryId, "Whole Milk");

        var result = await handler.HandleAsync(cmd, CancellationToken.None);

        var product = await DbContext.Products.AsNoTracking().SingleOrDefaultAsync(p => p.Id == productId);
        Assert.True(result.IsSuccess);
        Assert.NotNull(product);
        Assert.Equal("Whole Milk", product.Name);
        Assert.Equal(newCategoryId, product.CategoryId);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentProduct_ReturnsNotFound()
    {
        var userId = await SeedUserAsync("user@example.com");
        var categoryId = await SeedCategoryAsync(userId, "Dairy");
        var handler = new UpdateProductHandler(DbContext);
        var cmd = new UpdateProductCommand(userId, Guid.CreateVersion7(), categoryId, "Milk");

        var result = await handler.HandleAsync(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_WithOtherUserProduct_ReturnsNotFound()
    {
        var ownerUserId = await SeedUserAsync("owner@example.com");
        var attackerUserId = await SeedUserAsync("attacker@example.com");
        var ownerCategoryId = await SeedCategoryAsync(ownerUserId, "Dairy");
        var attackerCategoryId = await SeedCategoryAsync(attackerUserId, "Drinks");
        var productId = await SeedProductAsync(ownerUserId, ownerCategoryId, "Milk");
        var handler = new UpdateProductHandler(DbContext);
        var cmd = new UpdateProductCommand(attackerUserId, productId, attackerCategoryId, "Hijacked");

        var result = await handler.HandleAsync(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_WithCategoryOfOtherUser_ReturnsNotFound()
    {
        var userId = await SeedUserAsync("user@example.com");
        var otherUserId = await SeedUserAsync("other@example.com");
        var myCategoryId = await SeedCategoryAsync(userId, "Dairy");
        var otherCategoryId = await SeedCategoryAsync(otherUserId, "Foreign");
        var productId = await SeedProductAsync(userId, myCategoryId, "Milk");
        var handler = new UpdateProductHandler(DbContext);
        var cmd = new UpdateProductCommand(userId, productId, otherCategoryId, "Milk");

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
        var handler = new UpdateProductHandler(DbContext);
        var cmd = new UpdateProductCommand(Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            invalidName!);

        var result = await handler.HandleAsync(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorKind.Validation, result.Error.Kind);
    }
}
