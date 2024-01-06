using MyCosts.Postgres.Entities.Abstractions;

namespace MyCosts.Postgres.Entities;

public class ProductCategoryEntity : IPostgresEntity
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required int UserId { get; set; }
    public UserEntity? User { get; set; }

    public ICollection<ProductEntity> Products { get; set; } = new List<ProductEntity>();
}