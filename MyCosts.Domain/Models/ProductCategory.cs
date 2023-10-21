namespace MyCosts.Domain.Models;

public class ProductCategory
{
    public required int Id { get; set; }
    public required string Name { get; set; }

    public required int UserId { get; set; }
    public User? User { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}