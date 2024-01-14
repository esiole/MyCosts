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

public class ReceiptService : IReceiptService
{
    private readonly IReceiptRepository _receiptRepository;

    public ReceiptService(IReceiptRepository receiptRepository)
    {
        _receiptRepository = receiptRepository;
    }

    public async Task<Receipt> AddAsync(Receipt receipt) => await _receiptRepository.AddAsync(receipt);

    public async Task<Receipt?> DeleteAsync(int receiptId, User deleter)
    {
        var receipt = await _receiptRepository.GetAsync(receiptId, deleter.Id);
        if (receipt == null) return null;

        await _receiptRepository.DeleteAsync(receiptId);
        return receipt;
    }

    public async Task<Receipt?> EditAsync(Receipt receipt, User editor)
    {
        var origin = await _receiptRepository.GetAsync(receipt.Id, editor.Id);
        if (origin == null) return null;

        await _receiptRepository.UpdateAsync(receipt);
        return receipt;
    }

    public async Task<Receipt?> GetAsync(int receiptId, User requester, CancellationToken cancellationToken = default) =>
        await _receiptRepository.GetAsync(receiptId, requester.Id, cancellationToken);

    public async Task<ICollection<Receipt>> GetAsync(User requester, CancellationToken cancellationToken = default) =>
        await _receiptRepository.GetAsync(requester.Id, cancellationToken);
}