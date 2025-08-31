using BizMate.Domain.Entities;
using SqlKata.Execution;

namespace BizMate.Application.Common.Interfaces.Repositories
{
    public interface IExportReceiptRepository
    {
        Task<(List<ExportReceipt> Receipts, int TotalCount)> SearchReceiptsWithPaging(Guid storeId, DateTime? dateFrom, DateTime? dateTo, string? keyword, int pageIndex, int pageSize, QueryFactory queryFactory);
        Task AddAsync(ExportReceipt receipt, CancellationToken cancellationToken);
        Task<ExportReceipt?> GetByIdAsync(Guid id);
    }
}
