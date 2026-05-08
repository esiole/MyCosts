using Microsoft.AspNetCore.Identity;
using MyCosts.Application.Features.Auth.RegisterUser;
using MyCosts.Application.Features.ProductCategories.CreateProductCategory;
using MyCosts.Application.Features.ProductCategories.GetProductCategories;
using MyCosts.Domain.Users;
using MyCosts.IntegrationTests.Infrastructure;
using Xunit;

namespace MyCosts.IntegrationTests.Features.ProductCategories;

[Collection("Database")]
public class GetProductCategoriesHandlerTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    private readonly IPasswordHasher<User> _hasher = new PasswordHasher<User>();

    private async Task<Guid> SeedUserAsync(string email)
    {
        var handler = new RegisterUserHandler(DbContext, _hasher);
        var result = await handler.HandleAsync(new RegisterUserCommand(email, "Password1!"), CancellationToken.None);
        return result.Value;
    }

    private async Task SeedCategoryAsync(Guid userId, string name)
    {
        var handler = new CreateProductCategoryHandler(DbContext);
        await handler.HandleAsync(new CreateProductCategoryCommand(userId, name), CancellationToken.None);
    }

    [Fact]
    public async Task HandleAsync_ReturnsOnlyUserCategories()
    {
        var userAId = await SeedUserAsync("usera@example.com");
        var userBId = await SeedUserAsync("userb@example.com");
        await SeedCategoryAsync(userAId, "Dairy");
        await SeedCategoryAsync(userAId, "Meat");
        await SeedCategoryAsync(userBId, "Drinks");
        var handler = new GetProductCategoriesHandler(DbContext);
        var query = new GetProductCategoriesQuery(userAId);

        var result = await handler.HandleAsync(query, CancellationToken.None);

        Assert.Equal(2, result.Items.Count);
        Assert.All(result.Items, item => Assert.True(item.Name is "Dairy" or "Meat"));
    }

    [Fact]
    public async Task HandleAsync_ReturnsSortedAlphabetically()
    {
        var userId = await SeedUserAsync("usera@example.com");
        await SeedCategoryAsync(userId, "Zebra");
        await SeedCategoryAsync(userId, "Apple");
        await SeedCategoryAsync(userId, "Mango");
        var handler = new GetProductCategoriesHandler(DbContext);
        var query = new GetProductCategoriesQuery(userId);

        var result = await handler.HandleAsync(query, CancellationToken.None);

        var names = result.Items.Select(i => i.Name).ToList();
        Assert.Equal(["Apple", "Mango", "Zebra"], names);
    }

    [Fact]
    public async Task HandleAsync_WithSearch_FiltersResults()
    {
        var userId = await SeedUserAsync("usera@example.com");
        await SeedCategoryAsync(userId, "Dairy");
        await SeedCategoryAsync(userId, "Drinks");
        await SeedCategoryAsync(userId, "Meat");
        var handler = new GetProductCategoriesHandler(DbContext);
        var query = new GetProductCategoriesQuery(userId, Search: "d");

        var result = await handler.HandleAsync(query, CancellationToken.None);

        var names = result.Items.Select(i => i.Name).ToList();
        Assert.Equal(2, names.Count);
        Assert.Contains("Dairy", names);
        Assert.Contains("Drinks", names);
    }

    [Fact]
    public async Task HandleAsync_WithCursor_ReturnsNextPage()
    {
        var userId = await SeedUserAsync("usera@example.com");
        foreach (var name in new[] { "A", "B", "C", "D", "E" })
        {
            await SeedCategoryAsync(userId, name);
        }

        var handler = new GetProductCategoriesHandler(DbContext);

        var query1 = new GetProductCategoriesQuery(userId, Limit: 2);
        var page1 = await handler.HandleAsync(query1, CancellationToken.None);
        Assert.Equal(["A", "B"], page1.Items.Select(i => i.Name).ToList());
        Assert.NotNull(page1.NextCursor);

        var query2 = new GetProductCategoriesQuery(userId, Cursor: page1.NextCursor, Limit: 2);
        var page2 = await handler.HandleAsync(query2, CancellationToken.None);
        Assert.Equal(["C", "D"], page2.Items.Select(i => i.Name).ToList());
        Assert.NotNull(page2.NextCursor);

        var query3 = new GetProductCategoriesQuery(userId, Cursor: page2.NextCursor, Limit: 2);
        var page3 = await handler.HandleAsync(query3, CancellationToken.None);
        Assert.Equal(["E"], page3.Items.Select(i => i.Name).ToList());
        Assert.Null(page3.NextCursor);
    }

    [Fact]
    public async Task HandleAsync_WhenLessThanLimit_ReturnsNullCursor()
    {
        var userId = await SeedUserAsync("usera@example.com");
        await SeedCategoryAsync(userId, "Alpha");
        await SeedCategoryAsync(userId, "Beta");
        var handler = new GetProductCategoriesHandler(DbContext);
        var query = new GetProductCategoriesQuery(userId, Limit: 5);

        var result = await handler.HandleAsync(query, CancellationToken.None);

        Assert.Equal(2, result.Items.Count);
        Assert.Null(result.NextCursor);
    }
}
