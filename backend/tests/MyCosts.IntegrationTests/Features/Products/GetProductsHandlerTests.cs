using Microsoft.AspNetCore.Identity;
using MyCosts.Application.Features.Auth.RegisterUser;
using MyCosts.Application.Features.ProductCategories.CreateProductCategory;
using MyCosts.Application.Features.Products.CreateProduct;
using MyCosts.Application.Features.Products.GetProducts;
using MyCosts.Domain.Users;
using MyCosts.IntegrationTests.Infrastructure;
using Xunit;

namespace MyCosts.IntegrationTests.Features.Products;

[Collection("Database")]
public class GetProductsHandlerTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
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
    public async Task HandleAsync_ReturnsOnlyUserProducts()
    {
        var userAId = await SeedUserAsync("usera@example.com");
        var userBId = await SeedUserAsync("userb@example.com");
        var catA = await SeedCategoryAsync(userAId, "Dairy");
        var catB = await SeedCategoryAsync(userBId, "Drinks");
        await SeedProductAsync(userAId, catA, "Milk");
        await SeedProductAsync(userAId, catA, "Cheese");
        await SeedProductAsync(userBId, catB, "Juice");
        var handler = new GetProductsHandler(DbContext);

        var result = await handler.HandleAsync(new GetProductsQuery(userAId), CancellationToken.None);

        Assert.Equal(2, result.Items.Count);
        Assert.All(result.Items, item => Assert.True(item.Name is "Milk" or "Cheese"));
    }

    [Fact]
    public async Task HandleAsync_ReturnsSortedAlphabetically()
    {
        var userId = await SeedUserAsync("usera@example.com");
        var categoryId = await SeedCategoryAsync(userId, "Grocery");
        await SeedProductAsync(userId, categoryId, "Zebra");
        await SeedProductAsync(userId, categoryId, "Apple");
        await SeedProductAsync(userId, categoryId, "Mango");
        var handler = new GetProductsHandler(DbContext);

        var result = await handler.HandleAsync(new GetProductsQuery(userId), CancellationToken.None);

        var names = result.Items.Select(i => i.Name).ToList();
        Assert.Equal(["Apple", "Mango", "Zebra"], names);
    }

    [Fact]
    public async Task HandleAsync_IncludesCategoryInfo()
    {
        var userId = await SeedUserAsync("usera@example.com");
        var categoryId = await SeedCategoryAsync(userId, "Dairy");
        await SeedProductAsync(userId, categoryId, "Milk");
        var handler = new GetProductsHandler(DbContext);

        var result = await handler.HandleAsync(new GetProductsQuery(userId), CancellationToken.None);

        var item = result.Items.Single();
        Assert.Equal("Milk", item.Name);
        Assert.Equal(categoryId, item.CategoryId);
        Assert.Equal("Dairy", item.CategoryName);
    }

    [Fact]
    public async Task HandleAsync_WithSearch_FiltersResults()
    {
        var userId = await SeedUserAsync("usera@example.com");
        var categoryId = await SeedCategoryAsync(userId, "Grocery");
        await SeedProductAsync(userId, categoryId, "Milk");
        await SeedProductAsync(userId, categoryId, "Mango");
        await SeedProductAsync(userId, categoryId, "Cheese");
        var handler = new GetProductsHandler(DbContext);

        var result = await handler.HandleAsync(new GetProductsQuery(userId, Search: "m"), CancellationToken.None);

        var names = result.Items.Select(i => i.Name).ToList();
        Assert.Equal(2, names.Count);
        Assert.Contains("Milk", names);
        Assert.Contains("Mango", names);
    }

    [Fact]
    public async Task HandleAsync_WithCursor_ReturnsNextPage()
    {
        var userId = await SeedUserAsync("usera@example.com");
        var categoryId = await SeedCategoryAsync(userId, "Grocery");
        foreach (var name in new[] { "A", "B", "C", "D", "E" })
        {
            await SeedProductAsync(userId, categoryId, name);
        }

        var handler = new GetProductsHandler(DbContext);

        var query1 = new GetProductsQuery(userId, Limit: 2);
        var page1 = await handler.HandleAsync(query1, CancellationToken.None);
        Assert.Equal(["A", "B"], page1.Items.Select(i => i.Name).ToList());
        Assert.NotNull(page1.NextCursor);

        var query2 = new GetProductsQuery(userId, Cursor: page1.NextCursor, Limit: 2);
        var page2 = await handler.HandleAsync(query2, CancellationToken.None);
        Assert.Equal(["C", "D"], page2.Items.Select(i => i.Name).ToList());
        Assert.NotNull(page2.NextCursor);

        var query3 = new GetProductsQuery(userId, Cursor: page2.NextCursor, Limit: 2);
        var page3 = await handler.HandleAsync(query3, CancellationToken.None);
        Assert.Equal(["E"], page3.Items.Select(i => i.Name).ToList());
        Assert.Null(page3.NextCursor);
    }

    [Fact]
    public async Task HandleAsync_WithSearchCaseInsensitive_FiltersResults()
    {
        var userId = await SeedUserAsync("usera@example.com");
        var categoryId = await SeedCategoryAsync(userId, "Grocery");
        await SeedProductAsync(userId, categoryId, "Milk");
        await SeedProductAsync(userId, categoryId, "Mango");
        await SeedProductAsync(userId, categoryId, "Cheese");
        var handler = new GetProductsHandler(DbContext);

        var result = await handler.HandleAsync(new GetProductsQuery(userId, Search: "M"), CancellationToken.None);

        var names = result.Items.Select(i => i.Name).ToList();
        Assert.Equal(2, names.Count);
        Assert.Contains("Milk", names);
        Assert.Contains("Mango", names);
    }

    [Fact]
    public async Task HandleAsync_WhenLessThanLimit_ReturnsNullCursor()
    {
        var userId = await SeedUserAsync("usera@example.com");
        var categoryId = await SeedCategoryAsync(userId, "Grocery");
        await SeedProductAsync(userId, categoryId, "Apple");
        await SeedProductAsync(userId, categoryId, "Banana");
        var handler = new GetProductsHandler(DbContext);

        var result = await handler.HandleAsync(new GetProductsQuery(userId, Limit: 5), CancellationToken.None);

        Assert.Equal(2, result.Items.Count);
        Assert.Null(result.NextCursor);
    }
}
