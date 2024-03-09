using MyCosts.Domain.Dto.Filters;
using MyCosts.Domain.Models;
using MyCosts.Domain.Services;

namespace MyCosts.Application.Services;

public interface IProductService
{
    Task<Product> AddAsync(Product product);
    Task<Product?> DeleteAsync(int productId, User deleter);
    Task<Product?> EditAsync(Product product, User editor);
    Task<Product?> GetAsync(int productId, User requester, CancellationToken cancellationToken = default);
    Task<ICollection<Product>> GetAsync(ProductFilter filter, User requester, CancellationToken cancellationToken = default);
}

public class ProductService(IProductRepository productRepository) : IProductService
{
    public async Task<Product> AddAsync(Product product) => await productRepository.AddAsync(product);

    public async Task<Product?> DeleteAsync(int productId, User deleter)
    {
        var product = await productRepository.GetAsync(productId, deleter.Id);
        if (product == null) return null;

        await productRepository.DeleteAsync(productId);
        return product;
    }

    public async Task<Product?> EditAsync(Product product, User editor)
    {
        var origin = await productRepository.GetAsync(product.Id, editor.Id);
        if (origin == null) return null;

        await productRepository.UpdateAsync(product);
        return product;
    }

    public async Task<Product?> GetAsync(int productId, User requester, CancellationToken cancellationToken = default) =>
        await productRepository.GetAsync(productId, requester.Id, cancellationToken);

    public async Task<ICollection<Product>> GetAsync(ProductFilter filter, User requester, CancellationToken cancellationToken = default) =>
        await productRepository.GetAsync(filter, requester.Id, cancellationToken);
}