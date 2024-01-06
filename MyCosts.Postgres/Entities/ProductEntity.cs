using MyCosts.Postgres.Entities.Abstractions;

namespace MyCosts.Postgres.Entities;

public class ProductEntity : IPostgresEntity
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required int CategoryId { get; set; }
    public ProductCategoryEntity? Category { get; set; }
}