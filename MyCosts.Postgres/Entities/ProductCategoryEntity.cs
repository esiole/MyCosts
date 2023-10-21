namespace MyCosts.Postgres.Entities;

public class ProductCategoryEntity
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required int UserId { get; set; }
    public UserEntity? User { get; set; }

    public ICollection<ProductEntity> Products { get; set; } = new List<ProductEntity>();
}