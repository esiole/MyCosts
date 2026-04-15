using Microsoft.EntityFrameworkCore.Storage;
using MyCosts.Infrastructure.Persistence;
using Xunit;

namespace MyCosts.IntegrationTests.Infrastructure;

[Collection("Database")]
public abstract class IntegrationTestBase(DatabaseFixture fixture) : IAsyncLifetime
{
    protected AppDbContext DbContext { get; private set; } = null!;
    private IDbContextTransaction _transaction = null!;

    public async Task InitializeAsync()
    {
        DbContext = fixture.CreateDbContext();
        _transaction = await DbContext.Database.BeginTransactionAsync();
    }

    public async Task DisposeAsync()
    {
        await _transaction.RollbackAsync();
        await DbContext.DisposeAsync();
    }
}

[CollectionDefinition("Database")]
public sealed class DatabaseCollection : ICollectionFixture<DatabaseFixture>;
