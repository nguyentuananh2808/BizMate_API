using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.UserCases.Export.Queries.ExportOrders;
using BizMate.Infrastructure.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;

namespace BizMate.Infrastructure.Persistence.Repositories
{
    public class ExportOrderRepository : IExportRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<ExportOrderRepository> _logger;

        public ExportOrderRepository(AppDbContext dbContext, ILogger<ExportOrderRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <summary>
        /// Lấy danh sách Order (và OrderDetail) theo khách hàng & khoảng thời gian.
        /// </summary>
        public async Task<ExportOrderCoreDto?> ExportOrderAsync(
            Guid storeId,
            ExportOrdersRequest exportOrder,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var query = _dbContext.Orders
                    .AsNoTracking()
                    .Where(o => o.StoreId == storeId
                                && o.CustomerId == exportOrder.CustomerId
                                //&& o.IsActive
                                && o.CreatedDate.Date >= exportOrder.DateFrom.Date
                                && o.CreatedDate.Date <= exportOrder.DateTo.Date)
                    .Include(o => o.Status)
                    .Include(o => o.Details)
                    .OrderByDescending(o => o.CreatedDate);

                var orders = await query.ToListAsync(cancellationToken);

                if (!orders.Any())
                    return new ExportOrderCoreDto { Orders = Array.Empty<OrderCoreDto>() };

                // Map sang DTO
                var orderDtos = orders.Select(o => new OrderCoreDto
                {
                    Id = o.Id,
                    Code = o.Code,
                    StoreId = o.StoreId,
                    CustomerId = o.CustomerId,
                    CustomerName = o.CustomerName,
                    CustomerPhone = o.CustomerPhone,
                    CustomerType = o.CustomerType,
                    DeliveryAddress = o.DeliveryAddress,
                    Description = o.Description,
                    StatusId = o.StatusId,
                    StatusName = o.Status.Name,
                    TotalAmount = o.TotalAmount,
                    IsActive = o.IsActive,
                    RowVersion = o.RowVersion,
                    OrderDate = o.CreatedDate,
                    Details = o.Details.Select(d => new OrderDetailDto
                    {
                        Id = d.Id,
                        OrderId = d.OrderId,
                        ProductId = d.ProductId,
                        ProductName = d.ProductName,
                        ProductCode = d.ProductCode,
                        Unit = d.Unit,
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice,
                        Available = d.Available,
                        Total = d.Total,
                        RowVersion = d.RowVersion
                    }).ToList()
                }).ToList();

                // Recalculate total để đảm bảo chính xác
                foreach (var o in orderDtos)
                    o.RecalculateTotal();

                return new ExportOrderCoreDto { Orders = orderDtos };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Lỗi khi thực hiện ExportOrderAsync (CustomerId={CustomerId}, StoreId={StoreId})",
                    exportOrder.CustomerId, storeId);
                throw;
            }

        }
    }
}
