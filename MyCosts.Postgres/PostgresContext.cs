using Microsoft.EntityFrameworkCore;
using MyCosts.Postgres.Entities;

namespace MyCosts.Postgres;

public class PostgresContext : DbContext
{
    public required DbSet<UserEntity> Users { get; init; }
    public required DbSet<ProductCategoryEntity> ProductCategories { get; init; }
    public required DbSet<ProductEntity> Products { get; init; }

    public PostgresContext()
    {
    }

    public PostgresContext(DbContextOptions options) : base(options)
    {
    }
}