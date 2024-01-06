using MyCosts.Postgres.Entities.Abstractions;

namespace MyCosts.Postgres.Entities;

public class UserEntity : IPostgresEntity
{
    public int Id { get; set; }

    public required string Email { get; set; }
    public required string Password { get; set; }

    public ICollection<ProductCategoryEntity> ProductCategories { get; set; } = new List<ProductCategoryEntity>();
    public ICollection<ReceiptEntity> Receipts { get; set; } = new List<ReceiptEntity>();
}