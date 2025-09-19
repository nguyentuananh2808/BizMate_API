using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Domain.Entities;
using SqlKata.Execution;

namespace BizMate.Application.Common.Interfaces.Repositories
{
    public interface IImportReceiptRepository
    {
        Task UpdateAsync(ImportReceipt receipt, CancellationToken cancellationToken);
        Task UpdateStatusAsync(UpdateImportReceiptStatusDto updateImportReceiptStatusDto, CancellationToken cancellationToken);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
        Task<List<ImportReceipt>> SearchReceipts(Guid storeId, int? type, string? keyword, QueryFactory queryFactory);
        Task<(List<ImportReceipt> Receipts, int TotalCount)> SearchReceiptsWithPaging(Guid storeId, DateTime? dateFrom, DateTime? dateTo, IEnumerable<Guid>? statusIds, string? keyword, int pageIndex, int pageSize, QueryFactory queryFactory);
        Task AddAsync(ImportReceipt receipt, CancellationToken cancellationToken);
        Task<ImportReceipt?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task UpdateWithDetailsAsync(
            ImportReceipt receipt,
            IEnumerable<ImportReceiptDetail> details,
            CancellationToken cancellationToken);
    }
}
