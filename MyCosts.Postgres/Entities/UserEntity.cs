using MyCosts.Postgres.Entities.Abstractions;

namespace MyCosts.Postgres.Entities;

public class UserEntity : IPostgresEntity
{
    public int Id { get; set; }

    public required string Email { get; set; }
    public required string Password { get; set; }

    public ICollection<ProductCategoryEntity> ProductCategories { get; set; } = [];
    public ICollection<ReceiptEntity> Receipts { get; set; } = [];
}