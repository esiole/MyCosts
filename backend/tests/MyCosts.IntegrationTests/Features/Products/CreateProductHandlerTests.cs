using Microsoft.AspNetCore.Identity;
using MyCosts.Application.Common;
using MyCosts.Application.Features.Auth.RegisterUser;
using MyCosts.Application.Features.ProductCategories.CreateProductCategory;
using MyCosts.Application.Features.Products.CreateProduct;
using MyCosts.Domain.Users;
using MyCosts.IntegrationTests.Infrastructure;
using Xunit;

namespace MyCosts.IntegrationTests.Features.Products;

[Collection("Database")]
public class CreateProductHandlerTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
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

    [Fact]
    public async Task HandleAsync_WithValidData_CreatesProduct()
    {
        var userId = await SeedUserAsync("user@example.com");
        var categoryId = await SeedCategoryAsync(userId, "Dairy");
        var handler = new CreateProductHandler(DbContext);
        var cmd = new CreateProductCommand(userId, categoryId, "Milk");

        var result = await handler.HandleAsync(cmd, CancellationToken.None);

        var product = await DbContext.Products.FindAsync(result.Value);
        Assert.True(result.IsSuccess);
        Assert.NotNull(product);
        Assert.Equal("Milk", product.Name);
        Assert.Equal(categoryId, product.CategoryId);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentCategory_ReturnsNotFound()
    {
        var handler = new CreateProductHandler(DbContext);
        var cmd = new CreateProductCommand(Guid.CreateVersion7(), Guid.CreateVersion7(), "Milk");

        var result = await handler.HandleAsync(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_WithOtherUserCategory_ReturnsNotFound()
    {
        var ownerUserId = await SeedUserAsync("owner@example.com");
        var attackerUserId = await SeedUserAsync("attacker@example.com");
        var categoryId = await SeedCategoryAsync(ownerUserId, "Dairy");
        var handler = new CreateProductHandler(DbContext);
        var cmd = new CreateProductCommand(attackerUserId, categoryId, "Milk");

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
        var handler = new CreateProductHandler(DbContext);
        var cmd = new CreateProductCommand(Guid.CreateVersion7(), Guid.CreateVersion7(), invalidName!);

        var result = await handler.HandleAsync(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorKind.Validation, result.Error.Kind);
    }
}
