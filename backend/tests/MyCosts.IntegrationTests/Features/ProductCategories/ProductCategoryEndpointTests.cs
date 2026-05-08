using System.Net;
using System.Net.Http.Json;
using MyCosts.IntegrationTests.Infrastructure;
using Xunit;

namespace MyCosts.IntegrationTests.Features.ProductCategories;

[Collection("App")]
public class ProductCategoryEndpointTests(AppFixture fixture)
{
    private const string BaseUrl = "/api/v1/categories";

    private async Task<HttpClient> CreateAuthenticatedClientAsync()
    {
        var client = fixture.CreateClient();
        var email = $"cat-{Guid.NewGuid()}@example.com";
        await client.PostAsJsonAsync("/api/v1/auth/register", new { email, password = "Password123!" });
        await client.PostAsJsonAsync("/api/v1/auth/login", new { email, password = "Password123!" });
        return client;
    }

    private static async Task<Guid> CreateCategoryAsync(HttpClient client, string name)
    {
        var response = await client.PostAsJsonAsync(BaseUrl, new { name });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<CategoryCreatedResponse>();
        return body!.Id;
    }

    [Fact]
    public async Task POST_WithValidData_Returns201()
    {
        var client = await CreateAuthenticatedClientAsync();

        var response = await client.PostAsJsonAsync(BaseUrl, new { name = "Dairy" });

        var body = await response.Content.ReadFromJsonAsync<CategoryCreatedResponse>();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotEqual(Guid.Empty, body!.Id);
    }

    [Fact]
    public async Task POST_WithDuplicateName_Returns409()
    {
        var client = await CreateAuthenticatedClientAsync();
        await client.PostAsJsonAsync(BaseUrl, new { name = "Dairy" });

        var response = await client.PostAsJsonAsync(BaseUrl, new { name = "Dairy" });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task POST_Unauthenticated_Returns401()
    {
        var client = fixture.CreateClient();

        var response = await client.PostAsJsonAsync(BaseUrl, new { name = "Dairy" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PUT_WithValidData_Returns204()
    {
        var client = await CreateAuthenticatedClientAsync();
        var id = await CreateCategoryAsync(client, "Dairy");

        var response = await client.PutAsJsonAsync($"{BaseUrl}/{id}", new { name = "Updated" });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task PUT_WithNonExistentId_Returns404()
    {
        var client = await CreateAuthenticatedClientAsync();

        var response = await client.PutAsJsonAsync($"{BaseUrl}/{Guid.NewGuid()}", new { name = "Updated" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DELETE_WithExistingCategory_Returns204()
    {
        var client = await CreateAuthenticatedClientAsync();
        var id = await CreateCategoryAsync(client, "Dairy");

        var response = await client.DeleteAsync($"{BaseUrl}/{id}");

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
    public async Task GET_ReturnsPaginatedCategories()
    {
        var client = await CreateAuthenticatedClientAsync();
        await CreateCategoryAsync(client, $"Alpha-{Guid.NewGuid():N}");
        await CreateCategoryAsync(client, $"Beta-{Guid.NewGuid():N}");
        await CreateCategoryAsync(client, $"Gamma-{Guid.NewGuid():N}");

        var response = await client.GetAsync($"{BaseUrl}?limit=2");

        var page = await response.Content.ReadFromJsonAsync<CursorPageResponse>();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(2, page!.Items.Count);
        Assert.NotNull(page.NextCursor);
    }

    [Fact]
    public async Task GET_WithSearch_FiltersResults()
    {
        var client = await CreateAuthenticatedClientAsync();
        await CreateCategoryAsync(client, "Dairy");
        await CreateCategoryAsync(client, "Drinks");
        await CreateCategoryAsync(client, "Meat");

        var response = await client.GetAsync($"{BaseUrl}?search=d");

        var page = await response.Content.ReadFromJsonAsync<CursorPageResponse>();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(2, page!.Items.Count);
        Assert.All(page.Items, item => Assert.StartsWith("d", item.Name, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task PUT_Unauthenticated_Returns401()
    {
        var client = fixture.CreateClient();

        var response = await client.PutAsJsonAsync($"{BaseUrl}/{Guid.NewGuid()}", new { name = "X" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DELETE_Unauthenticated_Returns401()
    {
        var client = fixture.CreateClient();

        var response = await client.DeleteAsync($"{BaseUrl}/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GET_Unauthenticated_Returns401()
    {
        var client = fixture.CreateClient();

        var response = await client.GetAsync(BaseUrl);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PUT_WithDuplicateName_Returns409()
    {
        var client = await CreateAuthenticatedClientAsync();
        await CreateCategoryAsync(client, "Alpha");
        var secondId = await CreateCategoryAsync(client, "Beta");

        var response = await client.PutAsJsonAsync($"{BaseUrl}/{secondId}", new { name = "Alpha" });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
}

file record CategoryCreatedResponse(Guid Id);

file record CategoryItemResponse(Guid Id, string Name);

file record CursorPageResponse(List<CategoryItemResponse> Items, string? NextCursor);
