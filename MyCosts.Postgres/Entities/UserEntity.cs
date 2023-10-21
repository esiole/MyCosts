namespace MyCosts.Postgres.Entities;

public class UserEntity
{
    public int Id { get; set; }

    public required string Email { get; set; }
    public required string Password { get; set; }

    public ICollection<ProductCategoryEntity> ProductCategories = new List<ProductCategoryEntity>();
}