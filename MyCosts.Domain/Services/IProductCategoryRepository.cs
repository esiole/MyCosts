using MyCosts.Domain.Dto.Filters;
using MyCosts.Domain.Models;

namespace MyCosts.Domain.Services;

public interface IProductCategoryRepository
{
    Task<ProductCategory> AddAsync(ProductCategory category);
    Task DeleteAsync(int categoryId);
    Task<ProductCategory?> GetAsync(int categoryId, int? requesterUserId, CancellationToken cancellationToken = default);
    Task<ICollection<ProductCategory>> GetAsync(ProductCategoryFilter filter, int? requesterUserId, CancellationToken cancellationToken = default);
    Task<ProductCategory?> UpdateAsync(ProductCategory category);
}