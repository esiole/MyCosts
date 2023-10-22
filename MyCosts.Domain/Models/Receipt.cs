namespace MyCosts.Domain.Models;

public class Receipt
{
    public required int Id { get; set; }

    public required DateOnly Date { get; set; }
    public required string PlaceName { get; set; }

    public required int UserId { get; set; }

    public ICollection<Cost> Costs { get; set; } = new List<Cost>();
}