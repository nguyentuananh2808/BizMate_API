using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Domain.Entities;
using SqlKata.Execution;

namespace BizMate.Application.Common.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        /// <summary>
        /// Thêm mới một đơn hàng
        /// </summary>
        Task AddAsync(Order order, CancellationToken cancellationToken = default);

        /// <summary>
        /// Cập nhật thông tin đơn hàng (không save changes ngay)
        /// </summary>
        Task UpdateWithDetailsAsync(OrderCoreDto order, IEnumerable<OrderDetail> details, CancellationToken cancellationToken = default);

        /// <summary>
        /// Cập nhật trạng thái đơn hàng nhanh (không load toàn bộ entity)
        /// </summary>
        Task UpdateStatusAsync(UpdateOrderStatusDto updateOrderStatusDto, CancellationToken cancellationToken = default);

        /// <summary>
        /// Xóa mềm đơn hàng
        /// </summary>
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lấy đơn hàng theo Id, kèm option include chi tiết
        /// </summary>
        Task<OrderCoreDto?> GetByIdAsync(Guid id, bool includeDetails = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lấy nhiều đơn hàng theo danh sách Ids
        /// </summary>
       // Task<List<Order>> GetByIdsAsync(IEnumerable<Guid> ids, bool includeDetails = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tìm kiếm đơn hàng cơ bản (không phân trang)
        /// </summary>
        Task<List<Order>> SearchReceipts(
            Guid storeId,
            int? type,
            string? keyword,
            QueryFactory queryFactory,
            CancellationToken cancellationToken = default);

        Task UpdateOrderAsync(OrderCoreDto? order, CancellationToken cancellationToken);

        /// <summary>
        /// Tìm kiếm đơn hàng có phân trang
        /// </summary>
        Task<(List<OrderCoreDto> Receipts, int TotalCount)> SearchReceiptsWithPaging(
            Guid storeId,
            DateTime? dateFrom,
            DateTime? dateTo,
            IEnumerable<Guid>? statusIds,
            string? keyword,
            int pageIndex,
            int pageSize,
            QueryFactory queryFactory,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Lưu tất cả thay đổi
        /// </summary>
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
