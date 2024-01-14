using Microsoft.EntityFrameworkCore;
using MyCosts.Domain.Models;
using MyCosts.Domain.Services;
using MyCosts.Postgres.Entities;
using MyCosts.Postgres.Mapping.Abstractions;
using MyCosts.Postgres.Repositories.Abstractions;

namespace MyCosts.Postgres.Repositories;

public class ReceiptRepository : PostgresRepository<ReceiptEntity, Receipt>, IReceiptRepository
{
    private readonly PostgresContext _postgresContext;

    protected override string[] HardLinkedCollections => new[] { nameof(ReceiptEntity.Costs) };

    public ReceiptRepository(PostgresContext postgresContext, IEntityMapper<ReceiptEntity, Receipt> mapper)
        : base(postgresContext, mapper)
    {
        _postgresContext = postgresContext;
    }

    public async Task<Receipt?> GetAsync(int receiptId, int? requesterUserId, CancellationToken cancellationToken = default)
    {
        var receipt = await _postgresContext.Receipts
            .Include(r => r.Costs)
            .FirstOrDefaultAsync(r =>
                    (!requesterUserId.HasValue || r.UserId == requesterUserId.Value) &&
                    receiptId == r.Id,
                cancellationToken);

        return receipt == null ? null : Mapper.MapToDomainModel(receipt);
    }

    public async Task<ICollection<Receipt>> GetAsync(int? requesterUserId, CancellationToken cancellationToken = default)
    {
        var receipts = await _postgresContext.Receipts
            .Include(r => r.Costs)
            .Where(r => !requesterUserId.HasValue || r.UserId == requesterUserId.Value)
            .ToListAsync(cancellationToken);

        return receipts.ConvertAll(Mapper.MapToDomainModel);
    }
}