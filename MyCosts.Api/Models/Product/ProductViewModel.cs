namespace MyCosts.Api.Models.Product;

/// <summary>
///     Model for viewing product
/// </summary>
public class ProductViewModel
{
    /// <summary>
    ///     Product identifier
    /// </summary>
    /// <example>23</example>
    public int Id { get; init; }

    /// <summary>
    ///     Product category name
    /// </summary>
    /// <example>Tomatoes</example>
    public required string Name { get; init; }

    /// <summary>
    ///     Identifier of the category to which the product belongs
    /// </summary>
    /// <example>168</example>
    public required int CategoryId { get; init; }
}