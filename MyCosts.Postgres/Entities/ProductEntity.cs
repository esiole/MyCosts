namespace MyCosts.Postgres.Entities;

public class ProductEntity
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required int CategoryId { get; set; }
    public ProductCategoryEntity? Category { get; set; }
}