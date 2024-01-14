using MyCosts.Domain.Models;

namespace MyCosts.Domain.Services;

public interface IReceiptRepository
{
    Task<Receipt> AddAsync(Receipt receipt);
    Task DeleteAsync(int receiptId);
    Task<Receipt?> GetAsync(int receiptId, int? requesterUserId, CancellationToken cancellationToken = default);
    Task<ICollection<Receipt>> GetAsync(int? requesterUserId, CancellationToken cancellationToken = default);
    Task<Receipt?> UpdateAsync(Receipt receipt);
}