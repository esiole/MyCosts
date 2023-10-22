namespace MyCosts.Postgres.Entities;

public class ReceiptEntity
{
    public int Id { get; set; }

    public required DateOnly Date { get; set; }
    public required string PlaceName { get; set; }

    public required int UserId { get; set; }
    public UserEntity? User { get; set; }

    public ICollection<CostEntity> Costs { get; set; } = new List<CostEntity>();
}