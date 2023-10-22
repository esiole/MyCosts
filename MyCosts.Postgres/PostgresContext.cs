using Microsoft.EntityFrameworkCore;
using MyCosts.Postgres.Entities;

namespace MyCosts.Postgres;

public class PostgresContext : DbContext
{
    public required DbSet<UserEntity> Users { get; init; }
    public required DbSet<ProductCategoryEntity> ProductCategories { get; init; }
    public required DbSet<ProductEntity> Products { get; init; }
    public required DbSet<ReceiptEntity> Receipts { get; init; }
    public required DbSet<CostEntity> Costs { get; init; }

    public PostgresContext()
    {
    }

    public PostgresContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CostEntity>().Property(c => c.Count).HasDefaultValue(1);
    }
}