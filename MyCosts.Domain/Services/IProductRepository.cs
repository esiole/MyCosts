using MyCosts.Domain.Models;

namespace MyCosts.Domain.Services;

public interface IProductRepository
{
    Task<Product> AddAsync(Product product);
    Task DeleteAsync(int productId);
    Task<Product?> GetAsync(int productId, int? requesterUserId, CancellationToken cancellationToken = default);
    Task<ICollection<Product>> GetAsync(int? requesterUserId, CancellationToken cancellationToken = default);
    Task<Product?> UpdateAsync(Product product);
}