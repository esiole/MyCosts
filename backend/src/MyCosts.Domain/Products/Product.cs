namespace MyCosts.Domain.Products;

public sealed class Product
{
    public Guid Id { get; init; }
    public Guid CategoryId { get; init; }
    public string Name { get; init; }

    public Product(Guid id, Guid categoryId, string name)
    {
        Id = id;
        CategoryId = categoryId;
        Name = name;
    }
}
