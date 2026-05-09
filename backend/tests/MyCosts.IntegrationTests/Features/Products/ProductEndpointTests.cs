using System.Net;
using System.Net.Http.Json;
using MyCosts.IntegrationTests.Infrastructure;
using Xunit;

namespace MyCosts.IntegrationTests.Features.Products;

[Collection("App")]
public class ProductEndpointTests(AppFixture fixture)
{
    private const string BaseUrl = "/api/v1/products";
    private const string CategoriesUrl = "/api/v1/categories";

    private async Task<HttpClient> CreateAuthenticatedClientAsync()
    {
        var client = fixture.CreateClient();
        var email = $"prod-{Guid.NewGuid()}@example.com";
        await client.PostAsJsonAsync("/api/v1/auth/register", new { email, password = "Password123!" });
        await client.PostAsJsonAsync("/api/v1/auth/login", new { email, password = "Password123!" });
        return client;
    }

    private static async Task<Guid> CreateCategoryAsync(HttpClient client, string name)
    {
        var response = await client.PostAsJsonAsync(CategoriesUrl, new { name });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<CategoryCreatedResponse>();
        return body!.Id;
    }

    private static async Task<Guid> CreateProductAsync(HttpClient client, Guid categoryId, string name)
    {
        var response = await client.PostAsJsonAsync(BaseUrl, new { categoryId, name });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<ProductCreatedResponse>();
        return body!.Id;
    }

    [Fact]
    public async Task POST_WithValidData_Returns201()
    {
        var client = await CreateAuthenticatedClientAsync();
        var categoryId = await CreateCategoryAsync(client, $"Cat-{Guid.NewGuid():N}");

        var response = await client.PostAsJsonAsync(BaseUrl, new { categoryId, name = "Milk" });

        var body = await response.Content.ReadFromJsonAsync<ProductCreatedResponse>();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotEqual(Guid.Empty, body!.Id);
    }

    [Fact]
    public async Task POST_WithNonExistentCategory_Returns404()
    {
        var client = await CreateAuthenticatedClientAsync();

        var response = await client.PostAsJsonAsync(BaseUrl, new { categoryId = Guid.NewGuid(), name = "Milk" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task POST_Unauthenticated_Returns401()
    {
        var client = fixture.CreateClient();

        var response = await client.PostAsJsonAsync(BaseUrl, new { categoryId = Guid.NewGuid(), name = "Milk" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PUT_WithValidData_Returns204()
    {
        var client = await CreateAuthenticatedClientAsync();
        var categoryId = await CreateCategoryAsync(client, $"Cat-{Guid.NewGuid():N}");
        var productId = await CreateProductAsync(client, categoryId, "Milk");

        var response = await client.PutAsJsonAsync($"{BaseUrl}/{productId}", new { categoryId, name = "Whole Milk" });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task PUT_WithNonExistentId_Returns404()
    {
        var client = await CreateAuthenticatedClientAsync();
        var categoryId = await CreateCategoryAsync(client, $"Cat-{Guid.NewGuid():N}");

        var response = await client.PutAsJsonAsync($"{BaseUrl}/{Guid.NewGuid()}", new { categoryId, name = "Milk" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PUT_Unauthenticated_Returns401()
    {
        var client = fixture.CreateClient();
        var request = new { categoryId = Guid.NewGuid(), name = "X" };

        var response = await client.PutAsJsonAsync($"{BaseUrl}/{Guid.NewGuid()}", request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DELETE_WithExistingProduct_Returns204()
    {
        var client = await CreateAuthenticatedClientAsync();
        var categoryId = await CreateCategoryAsync(client, $"Cat-{Guid.NewGuid():N}");
        var productId = await CreateProductAsync(client, categoryId, "Milk");

        var response = await client.DeleteAsync($"{BaseUrl}/{productId}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DELETE_WithNonExistentId_Returns404()
    {
        var client = await CreateAuthenticatedClientAsync();

        var response = await client.DeleteAsync($"{BaseUrl}/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DELETE_Unauthenticated_Returns401()
    {
        var client = fixture.CreateClient();

        var response = await client.DeleteAsync($"{BaseUrl}/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GET_ReturnsPaginatedProducts()
    {
        var client = await CreateAuthenticatedClientAsync();
        var categoryId = await CreateCategoryAsync(client, $"Cat-{Guid.NewGuid():N}");
        await CreateProductAsync(client, categoryId, $"Alpha-{Guid.NewGuid():N}");
        await CreateProductAsync(client, categoryId, $"Beta-{Guid.NewGuid():N}");
        await CreateProductAsync(client, categoryId, $"Gamma-{Guid.NewGuid():N}");

        var response = await client.GetAsync($"{BaseUrl}?limit=2");

        var page = await response.Content.ReadFromJsonAsync<ProductPageResponse>();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(2, page!.Items.Count);
        Assert.NotNull(page.NextCursor);
    }

    [Fact]
    public async Task GET_WithSearch_FiltersResults()
    {
        var client = await CreateAuthenticatedClientAsync();
        var categoryId = await CreateCategoryAsync(client, $"Cat-{Guid.NewGuid():N}");
        await CreateProductAsync(client, categoryId, "Milk");
        await CreateProductAsync(client, categoryId, "Mango");
        await CreateProductAsync(client, categoryId, "Cheese");

        var response = await client.GetAsync($"{BaseUrl}?search=m");

        var page = await response.Content.ReadFromJsonAsync<ProductPageResponse>();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(2, page!.Items.Count);
        Assert.All(page.Items, item => Assert.StartsWith("m", item.Name, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task GET_IncludesCategoryInResponse()
    {
        var client = await CreateAuthenticatedClientAsync();
        var categoryId = await CreateCategoryAsync(client, "Dairy");
        await CreateProductAsync(client, categoryId, "Milk");

        var response = await client.GetAsync(BaseUrl);

        var page = await response.Content.ReadFromJsonAsync<ProductPageResponse>();
        var item = page!.Items.First(i => i.Name == "Milk");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(categoryId, item.Category.Id);
        Assert.Equal("Dairy", item.Category.Name);
    }

    [Fact]
    public async Task GET_Unauthenticated_Returns401()
    {
        var client = fixture.CreateClient();

        var response = await client.GetAsync(BaseUrl);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}

file record CategoryCreatedResponse(Guid Id);

file record ProductCreatedResponse(Guid Id);

file record CategoryInfo(Guid Id, string Name);

file record ProductItemResponse(Guid Id, string Name, CategoryInfo Category);

file record ProductPageResponse(List<ProductItemResponse> Items, string? NextCursor);
