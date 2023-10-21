namespace MyCosts.Domain.Models;

public class Product
{
    public required int Id { get; set; }

    public required string Name { get; set; }

    public required int CategoryId { get; set; }
    public ProductCategory? Category { get; set; }
}