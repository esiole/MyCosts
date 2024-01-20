using MyCosts.Domain.Models;
using MyCosts.Domain.Services;

namespace MyCosts.Application.Services;

public interface IReceiptService
{
    Task<Receipt> AddAsync(Receipt receipt);
    Task<Receipt?> DeleteAsync(int receiptId, User deleter);
    Task<Receipt?> EditAsync(Receipt receipt, User editor);
    Task<Receipt?> GetAsync(int receiptId, User requester, CancellationToken cancellationToken = default);
    Task<ICollection<Receipt>> GetAsync(User requester, CancellationToken cancellationToken = default);
}

public class ReceiptService(IReceiptRepository receiptRepository) : IReceiptService
{
    public async Task<Receipt> AddAsync(Receipt receipt) => await receiptRepository.AddAsync(receipt);

    public async Task<Receipt?> DeleteAsync(int receiptId, User deleter)
    {
        var receipt = await receiptRepository.GetAsync(receiptId, deleter.Id);
        if (receipt == null) return null;

        await receiptRepository.DeleteAsync(receiptId);
        return receipt;
    }

    public async Task<Receipt?> EditAsync(Receipt receipt, User editor)
    {
        var origin = await receiptRepository.GetAsync(receipt.Id, editor.Id);
        if (origin == null) return null;

        await receiptRepository.UpdateAsync(receipt);
        return receipt;
    }

    public async Task<Receipt?> GetAsync(int receiptId, User requester, CancellationToken cancellationToken = default) =>
        await receiptRepository.GetAsync(receiptId, requester.Id, cancellationToken);

    public async Task<ICollection<Receipt>> GetAsync(User requester, CancellationToken cancellationToken = default) =>
        await receiptRepository.GetAsync(requester.Id, cancellationToken);
}