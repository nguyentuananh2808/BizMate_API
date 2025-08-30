using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Domain.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SqlKata.Execution;

namespace BizMate.Application.Common.Interfaces.Repositories
{
    public interface IImportReceiptRepository
    {
        Task UpdateAsync(ImportReceipt receipt);
        Task UpdateStatusAsync(UpdateImportReceiptStatusDto updateImportReceiptStatusDto,CancellationToken cancellationToken);
        Task DeleteAsync(Guid id);
        Task<List<ImportReceipt>> SearchReceipts(Guid storeId, int? type, string? keyword, QueryFactory queryFactory);
        Task<(List<ImportReceipt> Receipts, int TotalCount)> SearchReceiptsWithPaging(Guid storeId, DateTime? dateFrom, DateTime? dateTo, string? statusCode, string? keyword, int pageIndex, int pageSize, QueryFactory queryFactory);
        Task AddAsync(ImportReceipt receipt, CancellationToken cancellationToken);
        Task<ImportReceipt?> GetByIdAsync(Guid id);
    }
}
