namespace MyCosts.Domain.Products;

public sealed class ProductCategory
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Name { get; init; }

    public ProductCategory(Guid id, Guid userId, string name)
    {
        Id = id;
        UserId = userId;
        Name = name;
    }
}
