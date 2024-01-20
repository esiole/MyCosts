using MyCosts.Domain.Dto.Filters;
using MyCosts.Domain.Models;
using MyCosts.Domain.Services;

namespace MyCosts.Application.Services;

public interface IProductCategoryService
{
    Task<ProductCategory> AddAsync(ProductCategory category);
    Task<ProductCategory?> DeleteAsync(int categoryId, User deleter);
    Task<ProductCategory?> EditAsync(ProductCategory category, User editor);
    Task<ProductCategory?> GetAsync(int categoryId, User requester, CancellationToken cancellationToken = default);
    Task<ICollection<ProductCategory>> GetAsync(ProductCategoryFilter filter, User requester, CancellationToken cancellationToken = default);
}

public class ProductCategoryService(IProductCategoryRepository productCategoryRepository) : IProductCategoryService
{
    public async Task<ProductCategory> AddAsync(ProductCategory category) =>
        await productCategoryRepository.AddAsync(category);

    public async Task<ProductCategory?> DeleteAsync(int categoryId, User deleter)
    {
        var category = await productCategoryRepository.GetAsync(categoryId, deleter.Id);
        if (category == null) return null;

        await productCategoryRepository.DeleteAsync(categoryId);
        return category;
    }

    public async Task<ProductCategory?> EditAsync(ProductCategory category, User editor)
    {
        var origin = await productCategoryRepository.GetAsync(category.Id, editor.Id);
        if (origin == null) return null;

        await productCategoryRepository.UpdateAsync(category);
        return category;
    }

    public async Task<ProductCategory?> GetAsync(int categoryId, User requester, CancellationToken cancellationToken = default) =>
        await productCategoryRepository.GetAsync(categoryId, requester.Id, cancellationToken);

    public async Task<ICollection<ProductCategory>> GetAsync(ProductCategoryFilter filter, User requester, CancellationToken cancellationToken = default) =>
        await productCategoryRepository.GetAsync(filter, requester.Id, cancellationToken);
}