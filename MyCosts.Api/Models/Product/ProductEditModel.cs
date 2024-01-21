namespace MyCosts.Api.Models.Product;

/// <summary>
///     Model for creating and editing product
/// </summary>
public class ProductEditModel
{
    /// <summary>
    ///     Product name
    /// </summary>
    /// <example>Tomatoes</example>
    public required string Name { get; init; }

    /// <summary>
    ///     Identifier of the category to which the product belongs
    /// </summary>
    /// <example>168</example>
    public required int CategoryId { get; init; }
}