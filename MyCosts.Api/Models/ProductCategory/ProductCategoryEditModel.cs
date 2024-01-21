namespace MyCosts.Api.Models.ProductCategory;

/// <summary>
///     Model for creating and editing product categories
/// </summary>
public class ProductCategoryEditModel
{
    /// <summary>
    ///     Product category name
    /// </summary>
    /// <example>Medicines</example>
    public required string Name { get; init; }
}