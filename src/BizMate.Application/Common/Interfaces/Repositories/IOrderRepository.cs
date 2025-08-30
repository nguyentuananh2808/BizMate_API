using BizMate.Domain.Entities;
using SqlKata.Execution;

namespace BizMate.Application.Common.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task UpdateAsync(Order receipt);
       // Task UpdateStatusAsync(UpdateOrderStatusDto updateOrderStatusDto, CancellationToken cancellationToken);
        Task DeleteAsync(Guid id);
        Task<List<Order>> SearchReceipts(Guid storeId, int? type, string? keyword, QueryFactory queryFactory);
        Task<(List<Order> Receipts, int TotalCount)> SearchReceiptsWithPaging(Guid storeId, DateTime? dateFrom, DateTime? dateTo, string? statusCode, string? keyword, int pageIndex, int pageSize, QueryFactory queryFactory);
        Task AddAsync(Order receipt, CancellationToken cancellationToken);
        Task<Order?> GetByIdAsync(Guid id);
    }
}
