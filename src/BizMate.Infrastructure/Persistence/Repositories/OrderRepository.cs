using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Domain.Entities;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SqlKata.Execution;
using System.Text.Json;

namespace BizMate.Infrastructure.Persistence.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;
        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        private Order MapToOrder(OrderCoreDto dto)
        {
            var order = new Order
            {
                Id = dto.Id,
                OrderDate = dto.OrderDate,
                CustomerType = dto.CustomerType,
                CustomerId = dto.CustomerId,
                CustomerName = dto.CustomerName,
                CustomerPhone = dto.CustomerPhone,
                DeliveryAddress = dto.DeliveryAddress,
                Description = dto.Description,
                StatusId = dto.StatusId,
                Status = dto.Status,
                RowVersion = dto.RowVersion,
                UpdatedBy = dto.UpdatedBy,
                Details = dto.Details?.Select(d => new OrderDetail
                {
                    Id = d.Id,
                    ProductId = d.ProductId,
                    ProductName = d.ProductName,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    Total = d.Total
                }).ToList() ?? new List<OrderDetail>()
            };

            // Tính lại TotalAmount từ Details
            order.RecalculateTotal();

            return order;
        }



        public async Task AddAsync(Order receipt, CancellationToken cancellationToken = default)
        {
            await _context.Orders.AddAsync(receipt, cancellationToken);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateWithDetailsAsync(OrderCoreDto orderDto, IEnumerable<OrderDetail> details, CancellationToken cancellationToken = default)
        {
            var order = MapToOrder(orderDto);
            // Lấy connection từ DbContext
            var conn = _context.Database.GetDbConnection();

            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync(cancellationToken);

            // Detach order + chi tiết khỏi EF Core tracking
            // để EF không can thiệp khi gọi store procedure
            _context.Entry(order).State = EntityState.Detached;
            foreach (var d in order.Details)
                _context.Entry(d).State = EntityState.Detached;

            // Serialize chi tiết thành JSON
            var detailsJson = JsonSerializer.Serialize(details);

            // Lấy transaction hiện tại nếu có (EF quản lý)
            var dbTransaction = _context.Database.CurrentTransaction?.GetDbTransaction();

            // Gọi store procedure bằng Dapper
            await conn.ExecuteAsync(
                @"CALL sp_update_order(
            @p_order_id,
            @p_customer_id,
            @p_customer_type,
            @p_customer_phone,
            @p_customer_name,
            @p_delivery_address,
            @p_description,
            @p_status_id,
            @p_row_version,
            @p_updated_by,
            @p_details::jsonb
        )",
                new
                {
                    p_order_id = order.Id,
                    p_customer_id = order.CustomerId,
                    p_customer_type = order.CustomerType,
                    p_customer_phone = order.CustomerPhone,
                    p_customer_name = order.CustomerName,
                    p_delivery_address = order.DeliveryAddress,
                    p_description = order.Description,
                    p_status_id = order.StatusId,
                    p_row_version = order.RowVersion,
                    p_updated_by = order.UpdatedBy,
                    p_details = detailsJson
                },
                dbTransaction
            );
        }


        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var receipt = await _context.Orders.Include(r => r.Details).FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted, cancellationToken);
            if (receipt != null)
            {
                receipt.IsDeleted = true;
                receipt.DeletedAt = DateTime.UtcNow;
                _context.Orders.Update(receipt);
            }
        }

        public async Task<OrderCoreDto?> GetByIdAsync(Guid id, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            var order = await _context.Orders
                .Where(r => r.Id == id && !r.IsDeleted)
                .Include(r => r.Details)
                .Include(r => r.Status)
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(cancellationToken);

            if (order == null) return null;

            var productIds = order.Details.Select(d => d.ProductId).ToList();

            var stocks = await _context.Stocks
                 .Where(s => productIds.Contains(s.ProductId))
                 .Select(s => new
                 {
                     s.ProductId,
                     Available = s.Quantity - s.Reserved
                 })
                 .ToDictionaryAsync(x => x.ProductId, x => x.Available, cancellationToken);

            var dto = new OrderCoreDto
            {
                Id = order.Id,
                Code = order.Code,
                OrderDate = order.OrderDate,
                CustomerType = order.CustomerType,
                CustomerId = order.CustomerId,
                Customer = order.Customer,
                CustomerName = order.CustomerName,
                CustomerPhone = order.CustomerPhone,
                DeliveryAddress = order.DeliveryAddress,
                TotalAmount = order.TotalAmount,
                StatusId = order.StatusId,
                Description = order.Description,
                StatusName = order.Status.Name,
                RowVersion = order.RowVersion,
                CreatedDate = order.CreatedDate,
                UpdatedDate = order.UpdatedDate,
                Status = order.Status,
                Details = order.Details.Select(d => new OrderDetailDto
                {
                    Id = d.Id,
                    ProductId = d.ProductId,
                    ProductCode = d.ProductCode,
                    ProductName = d.ProductName,
                    Quantity = d.Quantity,
                    Unit = d.Unit,
                    UnitPrice = d.UnitPrice,
                    Total = d.Total,
                    Available = stocks.TryGetValue(d.ProductId, out var available) ? available : 0
                }).ToList()
            };

            dto.RecalculateTotal();

            return dto;
        }


        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<Order>> SearchReceipts(Guid storeId, int? type, string? keyword, QueryFactory queryFactory, CancellationToken cancellationToken = default)
        {
            var query = queryFactory.Query("Orders as r")
                .Where("r.StoreId", storeId)
                .WhereFalse("r.IsDeleted");

            if (type.HasValue)
                query.Where("r.CustomerType", type.Value);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var keywordLower = keyword.ToLower();
                query.Where(q =>
                    q.WhereRaw(@"LOWER(r.""Code"") LIKE ?", $"%{keywordLower}%")
                     .OrWhereRaw(@"LOWER(r.""CustomerName"") LIKE ?", $"%{keywordLower}%")
                     .OrWhereRaw(@"LOWER(r.""CustomerPhone"") LIKE ?", $"%{keywordLower}%"));
            }

            var result = await query.GetAsync<Order>();
            return result.ToList();
        }

        public async Task<(List<OrderCoreDto> Receipts, int TotalCount)> SearchReceiptsWithPaging(
            Guid storeId,
            DateTime? dateFrom,
            DateTime? dateTo,
            IEnumerable<Guid>? statusIds,
            string? keyword,
            int pageIndex,
            int pageSize,
            QueryFactory queryFactory,
            CancellationToken cancellationToken)
        {
            var baseQuery = queryFactory.Query("Orders as r")
                .LeftJoin("Statuses as s", "r.StatusId", "s.Id")
                .Where("r.StoreId", storeId)
                .WhereFalse("r.IsDeleted");

            //  Search linh hoạt: bỏ dấu + lowercase
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var keywordLower = keyword.ToLower();

                baseQuery.Where(q =>
                    q.WhereRaw(@"LOWER(unaccent(r.""Code"")) LIKE unaccent(?)", $"%{keywordLower}%")
                     .OrWhereRaw(@"LOWER(unaccent(r.""CustomerName"")) LIKE unaccent(?)", $"%{keywordLower}%")
                     .OrWhereRaw(@"LOWER(unaccent(r.""CustomerPhone"")) LIKE unaccent(?)", $"%{keywordLower}%"));
            }

            // Filter ngày
            if (dateFrom.HasValue)
                baseQuery.Where("r.OrderDate", ">=", dateFrom.Value);

            if (dateTo.HasValue)
                baseQuery.Where("r.OrderDate", "<=", dateTo.Value);

            //  Filter status
            if (statusIds != null && statusIds.Any())
                baseQuery.WhereIn("s.Id", statusIds);

            // Tổng số record
            var totalCount = await baseQuery.Clone().CountAsync<int>();

            // Lấy danh sách có phân trang
            var rows = await baseQuery
                .Select("r.*", "s.Id as Status_Id", "s.Name as Status_Name", "s.Code as Status_Code")
                .OrderByDesc("r.Code")
                .Offset((pageIndex - 1) * pageSize)
                .Limit(pageSize)
                .GetAsync();

            var results = rows.Select(row => new OrderCoreDto
            {
                Id = row.Id,
                StoreId = row.StoreId,
                Code = row.Code,
                CustomerName = row.CustomerName,
                CustomerType = row.CustomerType,
                CustomerPhone = row.CustomerPhone,
                CustomerId = row.CustomerId,
                DeliveryAddress = row.DeliveryAddress,
                TotalAmount = row.TotalAmount,
                StatusId = row.StatusId,
                Description = row.Description,
                StatusName = row.Status_Name,
                CreatedDate = row.CreatedDate,
                UpdatedDate = row.UpdateDate
            }).ToList();

            return (results, totalCount);
        }



        public async Task UpdateStatusAsync(UpdateOrderStatusDto statusOrder, CancellationToken cancellationToken)
        {
            // Lưu ý: ExecuteUpdateAsync sẽ áp dụng trực tiếp trên DB; handler hiện tại dùng phương thức SaveChangesAsync chung nên
            // nếu bạn muốn dùng ExecuteUpdateAsync, phải hiểu hành vi transaction của EF. Ở đây vẫn giữ cho các thao tác do handler quản lý.
            await _context.Orders
                .Where(r => r.StoreId == statusOrder.StoreId && r.Id == statusOrder.Id && !r.IsDeleted)
                .ExecuteUpdateAsync(r => r
                    .SetProperty(x => x.StatusId, statusOrder.StatusId)
                    .SetProperty(x => x.RowVersion, statusOrder.RowVersion)
                    .SetProperty(x => x.UpdatedBy, statusOrder.UpdatedBy)
                    .SetProperty(x => x.UpdatedDate, statusOrder.UpdatedDate), cancellationToken);
        }

        public async Task UpdateOrderAsync(OrderCoreDto orderDto, CancellationToken cancellationToken)
        {
            // Lấy entity gốc từ DbContext (EF Core tracking)
            var trackedOrder = await _context.Orders
                .Include(o => o.Details)
                .FirstOrDefaultAsync(o => o.Id == orderDto.Id, cancellationToken);

            if (trackedOrder == null)
                throw new InvalidOperationException($"Order {orderDto.Id} không tồn tại.");

            // Cập nhật các trường cha
            trackedOrder.CustomerId = orderDto.CustomerId;
            trackedOrder.CustomerType = orderDto.CustomerType;
            trackedOrder.CustomerName = orderDto.CustomerName;
            trackedOrder.CustomerPhone = orderDto.CustomerPhone;
            trackedOrder.DeliveryAddress = orderDto.DeliveryAddress;
            trackedOrder.Description = orderDto.Description;
            trackedOrder.StatusId = orderDto.StatusId;
            trackedOrder.RowVersion = orderDto.RowVersion;
            trackedOrder.UpdatedBy = orderDto.UpdatedBy;
            trackedOrder.UpdatedDate = DateTime.UtcNow;

            // Không gọi _context.Update(trackedOrder) nữa, EF đang track sẵn
            await _context.SaveChangesAsync(cancellationToken);
        }


    }
}
