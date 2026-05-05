using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyCosts.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace MyCosts.IntegrationTests.Infrastructure;

public sealed class AppFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:18-alpine").Build();
    private WebApplicationFactory<Program> _factory = null!;

    public HttpClient CreateClient() => _factory.CreateClient();

    private AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_container.GetConnectionString())
            .UseSnakeCaseNamingConvention()
            .Options;

        return new AppDbContext(options);
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        await using var db = CreateDbContext();
        await db.Database.MigrateAsync();

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((_, cfg) =>
                {
                    cfg.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:DefaultConnection"] = _container.GetConnectionString(),
                        ["Jwt:SecretKey"] = "test-secret-key-minimum-32-characters-long!!",
                    });
                });
            });
    }

    public async Task DisposeAsync()
    {
        await _factory.DisposeAsync();
        await _container.DisposeAsync();
    }
}

[CollectionDefinition("App")]
public sealed class AppCollection : ICollectionFixture<AppFixture>;
