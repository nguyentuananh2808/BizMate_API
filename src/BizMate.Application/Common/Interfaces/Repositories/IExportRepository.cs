using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.UserCases.Export.Queries.ExportOrders;

namespace BizMate.Application.Common.Interfaces.Repositories
{
    public interface IExportRepository
    {

        /// <summary>
        /// Lấy danh sach đơn hàng để xuất file
        /// </summary>
        Task<ExportOrderCoreDto?> ExportOrderAsync(Guid storeId, ExportOrdersRequest exportOrder, CancellationToken cancellationToken = default);

    }
}
