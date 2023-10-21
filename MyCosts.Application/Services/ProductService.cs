using MyCosts.Domain.Models;
using MyCosts.Domain.Services;

namespace MyCosts.Application.Services;

public interface IProductService
{
    Task<Product> AddAsync(Product product);
    Task<Product?> DeleteAsync(int productId, User deleter);
    Task<Product?> EditAsync(Product product, User editor);
    Task<Product?> GetAsync(int productId, User requester, CancellationToken cancellationToken = default);
    Task<ICollection<Product>> GetAsync(User requester, CancellationToken cancellationToken = default);
}

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Product> AddAsync(Product product) => await _productRepository.AddAsync(product);

    public async Task<Product?> DeleteAsync(int productId, User deleter)
    {
        var product = await _productRepository.GetAsync(productId, deleter.Id);
        if (product == null) return null;

        await _productRepository.DeleteAsync(productId);
        return product;
    }

    public async Task<Product?> EditAsync(Product product, User editor)
    {
        var origin = await _productRepository.GetAsync(product.Id, editor.Id);
        if (origin == null) return null;

        await _productRepository.UpdateAsync(product);
        return product;
    }

    public async Task<Product?> GetAsync(int productId, User requester, CancellationToken cancellationToken = default) =>
        await _productRepository.GetAsync(productId, requester.Id, cancellationToken);

    public async Task<ICollection<Product>> GetAsync(User requester, CancellationToken cancellationToken = default) =>
        await _productRepository.GetAsync(requester.Id, cancellationToken);
}