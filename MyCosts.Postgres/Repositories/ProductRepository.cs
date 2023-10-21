using Microsoft.EntityFrameworkCore;
using MyCosts.Domain.Models;
using MyCosts.Domain.Services;
using MyCosts.Postgres.Mapping;

namespace MyCosts.Postgres.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly PostgresContext _postgresContext;

    public ProductRepository(PostgresContext postgresContext)
    {
        _postgresContext = postgresContext;
    }

    public async Task<Product> AddAsync(Product product)
    {
        var productEntity = _postgresContext.Products.Add(product.ToEntity()).Entity;
        await _postgresContext.SaveChangesAsync();
        return productEntity.ToDomainModel();
    }

    public async Task DeleteAsync(int productId) =>
        await _postgresContext.Products.Where(p => p.Id == productId).ExecuteDeleteAsync();

    public async Task<Product?> GetAsync(int productId, int? requesterUserId, CancellationToken cancellationToken = default)
    {
        var product = await _postgresContext.Products
            .FirstOrDefaultAsync(p =>
                    (!requesterUserId.HasValue || p.Category!.UserId == requesterUserId.Value) &&
                    productId == p.Id,
                cancellationToken);

        return product?.ToDomainModel();
    }

    public async Task<ICollection<Product>> GetAsync(int? requesterUserId, CancellationToken cancellationToken = default)
    {
        var products = await _postgresContext.Products
            .Where(p => !requesterUserId.HasValue || p.Category!.UserId == requesterUserId.Value)
            .ToListAsync(cancellationToken);

        return products.ConvertAll(e => e.ToDomainModel());
    }

    public async Task<Product?> UpdateAsync(Product product)
    {
        var origin = await _postgresContext.Products.FindAsync(product.Id);
        if (origin == null) return null;

        _postgresContext.Entry(origin).CurrentValues.SetValues(product.ToEntity());
        await _postgresContext.SaveChangesAsync();

        return product;
    }
}