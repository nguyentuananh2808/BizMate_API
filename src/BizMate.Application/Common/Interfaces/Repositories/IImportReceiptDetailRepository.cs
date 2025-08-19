using BizMate.Domain.Entities;

namespace BizMate.Application.Common.Interfaces.Repositories
{
    public interface IImportReceiptDetailRepository
    {
        Task<List<ImportReceiptDetail>> GetByReceiptIdAsync(Guid receiptId, CancellationToken cancellationToken = default);

        Task<ImportReceiptDetail?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task AddAsync(ImportReceiptDetail detail, CancellationToken cancellationToken = default);

        Task AddRangeAsync(IEnumerable<ImportReceiptDetail> details, CancellationToken cancellationToken = default);

        Task UpdateAsync(ImportReceiptDetail detail, CancellationToken cancellationToken = default);

        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

        Task DeleteRangeAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);

    }
}
