using Microsoft.EntityFrameworkCore;
using MyCosts.Domain.Models;
using MyCosts.Domain.Services;
using MyCosts.Postgres.Entities;
using MyCosts.Postgres.Mapping.Abstractions;
using MyCosts.Postgres.Repositories.Abstractions;

namespace MyCosts.Postgres.Repositories;

public class ProductRepository : PostgresRepository<ProductEntity, Product>, IProductRepository
{
    private readonly PostgresContext _postgresContext;

    public ProductRepository(PostgresContext postgresContext, IEntityMapper<ProductEntity, Product> mapper)
        : base(postgresContext, mapper)
    {
        _postgresContext = postgresContext;
    }

    public async Task<Product?> GetAsync(int productId, int? requesterUserId, CancellationToken cancellationToken = default)
    {
        var product = await _postgresContext.Products
            .FirstOrDefaultAsync(p =>
                    (!requesterUserId.HasValue || p.Category!.UserId == requesterUserId.Value) &&
                    productId == p.Id,
                cancellationToken);

        return product == null ? null : Mapper.MapToDomainModel(product);
    }

    public async Task<ICollection<Product>> GetAsync(int? requesterUserId, CancellationToken cancellationToken = default)
    {
        var products = await _postgresContext.Products
            .Where(p => !requesterUserId.HasValue || p.Category!.UserId == requesterUserId.Value)
            .ToListAsync(cancellationToken);

        return products.ConvertAll(Mapper.MapToDomainModel);
    }
}