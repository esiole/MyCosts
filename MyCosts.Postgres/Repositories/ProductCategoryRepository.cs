using Microsoft.EntityFrameworkCore;
using MyCosts.Domain.Dto.Filters;
using MyCosts.Domain.Models;
using MyCosts.Domain.Services;
using MyCosts.Postgres.Entities;
using MyCosts.Postgres.Mapping.Abstractions;
using MyCosts.Postgres.Repositories.Abstractions;

namespace MyCosts.Postgres.Repositories;

public class ProductCategoryRepository : PostgresRepository<ProductCategoryEntity, ProductCategory>, IProductCategoryRepository
{
    public ProductCategoryRepository(PostgresContext postgresContext, IEntityMapper<ProductCategoryEntity, ProductCategory> mapper)
        : base(postgresContext, mapper)
    {
    }

    public async Task<ProductCategory?> GetAsync(int categoryId, int? requesterUserId, CancellationToken cancellationToken = default)
    {
        var category = await EntitySet.FirstOrDefaultAsync(c =>
                (!requesterUserId.HasValue || c.UserId == requesterUserId.Value) &&
                categoryId == c.Id,
            cancellationToken);

        return category == null ? null : Mapper.MapToDomainModel(category);
    }

    public async Task<ICollection<ProductCategory>> GetAsync(ProductCategoryFilter filter, int? requesterUserId, CancellationToken cancellationToken = default)
    {
        var categories = await EntitySet
            .Include(c => c.Products)
            .Where(c => !requesterUserId.HasValue || c.UserId == requesterUserId.Value)
            .Where(c => string.IsNullOrWhiteSpace(filter.Name) || c.Name.Contains(filter.Name))
            .ToListAsync(cancellationToken);

        return categories.ConvertAll(Mapper.MapToDomainModel);
    }
}