using Microsoft.EntityFrameworkCore;
using MyCosts.Domain.Dto.Filters;
using MyCosts.Domain.Models;
using MyCosts.Domain.Services;
using MyCosts.Postgres.Mapping;

namespace MyCosts.Postgres.Repositories;

public class ProductCategoryRepository : IProductCategoryRepository
{
    private readonly PostgresContext _postgresContext;

    public ProductCategoryRepository(PostgresContext postgresContext)
    {
        _postgresContext = postgresContext;
    }

    public async Task<ProductCategory> AddAsync(ProductCategory category)
    {
        var categoryEntity = _postgresContext.ProductCategories.Add(category.ToEntity()).Entity;
        await _postgresContext.SaveChangesAsync();
        return categoryEntity.ToDomainModel();
    }

    public async Task DeleteAsync(int categoryId) =>
        await _postgresContext.ProductCategories.Where(c => c.Id == categoryId).ExecuteDeleteAsync();

    public async Task<ProductCategory?> GetAsync(int categoryId, int? requesterUserId, CancellationToken cancellationToken = default)
    {
        var category = await _postgresContext.ProductCategories
            .FirstOrDefaultAsync(c =>
                    (!requesterUserId.HasValue || c.UserId == requesterUserId.Value) &&
                    categoryId == c.Id,
                cancellationToken);

        return category?.ToDomainModel();
    }

    public async Task<ICollection<ProductCategory>> GetAsync(ProductCategoryFilter filter, int? requesterUserId, CancellationToken cancellationToken = default)
    {
        var categories = await _postgresContext.ProductCategories
            .Include(c => c.Products)
            .Where(c =>
                (!requesterUserId.HasValue || c.UserId == requesterUserId.Value) &&
                (string.IsNullOrWhiteSpace(filter.Name) || c.Name.Contains(filter.Name))
            )
            .ToListAsync(cancellationToken);

        return categories.ConvertAll(e => e.ToDomainModel());
    }

    public async Task<ProductCategory?> UpdateAsync(ProductCategory category)
    {
        var origin = await _postgresContext.ProductCategories.FindAsync(category.Id);
        if (origin == null) return null;

        _postgresContext.Entry(origin).CurrentValues.SetValues(category.ToEntity());
        await _postgresContext.SaveChangesAsync();

        return category;
    }
}