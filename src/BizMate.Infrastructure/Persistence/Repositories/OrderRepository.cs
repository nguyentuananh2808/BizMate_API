using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Domain.Entities;
using BizMate.Infrastructure.Migrations;
using Microsoft.EntityFrameworkCore;
using SqlKata.Execution;

namespace BizMate.Infrastructure.Persistence.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;
        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        private static List<OrderTechnicianDto> ToOrderTechnicianDtos(Order order)
        {
            var technicians = order.OrderTechnicians
                .Where(x => x.Technician != null)
                .OrderBy(x => x.AssignedAt)
                .Select(x => new OrderTechnicianDto
                {
                    TechnicianId = x.TechnicianId,
                    TechnicianName = x.Technician.Name,
                    Phone = x.Technician.Phone
                })
                .ToList();

            if (technicians.Count > 0)
                return technicians;

            return order.TechnicianId.HasValue && order.Technician != null
                ? new List<OrderTechnicianDto>
                {
                    new()
                    {
                        TechnicianId = order.TechnicianId.Value,
                        TechnicianName = order.Technician.Name,
                        Phone = order.Technician.Phone
                    }
                }
                : new List<OrderTechnicianDto>();
        }

        private async Task AttachTechniciansAsync(
            IEnumerable<OrderCoreDto> orders,
            CancellationToken cancellationToken)
        {
            var orderList = orders.ToList();
            var orderIds = orderList.Select(x => x.Id).ToList();
            if (orderIds.Count == 0)
                return;

            var mappings = await _context.OrderTechnicians
                .Include(x => x.Technician)
                .Where(x => orderIds.Contains(x.OrderId))
                .OrderBy(x => x.AssignedAt)
                .ToListAsync(cancellationToken);

            var mappedByOrder = mappings
                .GroupBy(x => x.OrderId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => new OrderTechnicianDto
                    {
                        TechnicianId = x.TechnicianId,
                        TechnicianName = x.Technician.Name,
                        Phone = x.Technician.Phone
                    }).ToList());

            var legacyTechnicianIds = orderList
                .Where(x => !mappedByOrder.ContainsKey(x.Id) && x.TechnicianId.HasValue)
                .Select(x => x.TechnicianId!.Value)
                .Distinct()
                .ToList();

            var legacyTechnicians = legacyTechnicianIds.Count == 0
                ? new Dictionary<Guid, Technician>()
                : await _context.Technicians
                    .Where(x => legacyTechnicianIds.Contains(x.Id))
                    .ToDictionaryAsync(x => x.Id, cancellationToken);

            foreach (var order in orderList)
            {
                if (mappedByOrder.TryGetValue(order.Id, out var technicians))
                {
                    order.Technicians = technicians;
                    continue;
                }

                if (order.TechnicianId.HasValue
                    && legacyTechnicians.TryGetValue(order.TechnicianId.Value, out var technician))
                {
                    order.Technicians = new List<OrderTechnicianDto>
                    {
                        new()
                        {
                            TechnicianId = technician.Id,
                            TechnicianName = technician.Name,
                            Phone = technician.Phone
                        }
                    };
                }
            }
        }



        public async Task AddAsync(Order receipt, CancellationToken cancellationToken = default)
        {
            await _context.Orders.AddAsync(receipt, cancellationToken);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateWithDetailsAsync(OrderCoreDto orderDto, IEnumerable<OrderDetail> details, CancellationToken cancellationToken = default)
        {
            var order = await _context.Orders
                .Include(x => x.Details)
                .FirstOrDefaultAsync(x => x.Id == orderDto.Id && !x.IsDeleted, cancellationToken);

            if (order == null)
                throw new InvalidOperationException($"Order {orderDto.Id} không tồn tại.");

            var now = DateTime.UtcNow;

            order.CustomerId = orderDto.CustomerId;
            order.CustomerType = orderDto.CustomerType;
            order.CustomerName = orderDto.CustomerName;
            order.CustomerPhone = orderDto.CustomerPhone;
            order.DeliveryAddress = orderDto.DeliveryAddress;
            order.TechnicianId = orderDto.TechnicianId;
            order.InstallationDate = orderDto.InstallationDate;
            order.TechnicianExportedAt = orderDto.TechnicianExportedAt;
            order.Description = orderDto.Description;
            order.StatusId = orderDto.StatusId;
            order.RowVersion = orderDto.RowVersion;
            order.UpdatedBy = orderDto.UpdatedBy;
            order.UpdatedDate = now;

            _context.OrderDetails.RemoveRange(order.Details);

            var newDetails = details.Select(d =>
            {
                d.OrderId = order.Id;
                d.Order = order;
                return d;
            }).ToList();

            await _context.OrderDetails.AddRangeAsync(newDetails, cancellationToken);
            order.Details = newDetails;
            order.RecalculateTotal();
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
                .AsNoTracking()
                .Where(r => r.Id == id && !r.IsDeleted)
                .Include(r => r.Details)
                    .ThenInclude(d => d.ProductItems)
                .Include(r => r.Status)
                .Include(r => r.Customer)
                .Include(r => r.Technician)
                .Include(r => r.OrderTechnicians)
                    .ThenInclude(x => x.Technician)
                .FirstOrDefaultAsync(cancellationToken);

            if (order == null) return null;

            var productIds = order.Details.Select(d => d.ProductId).Distinct().ToList();

            var stocks = await _context.Stocks
                 .Where(s => s.StoreId == order.StoreId
                          && productIds.Contains(s.ProductId)
                          && !s.IsDeleted)
                 .Select(s => new
                 {
                     s.ProductId,
                     Available = s.Quantity - s.Reserved
                 })
                 .ToDictionaryAsync(x => x.ProductId, x => x.Available, cancellationToken);

            var serialTrackedByProduct = await _context.Products
                .Where(x => productIds.Contains(x.Id) && !x.IsDeleted)
                .Select(x => new { x.Id, x.IsSerialTracked })
                .ToDictionaryAsync(x => x.Id, x => x.IsSerialTracked, cancellationToken);

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
                TechnicianId = order.TechnicianId,
                TechnicianName = order.Technician?.Name,
                Technicians = ToOrderTechnicianDtos(order),
                InstallationDate = order.InstallationDate,
                TechnicianExportedAt = order.TechnicianExportedAt,
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
                    IsSerialTracked = serialTrackedByProduct.GetValueOrDefault(d.ProductId),
                    Unit = d.Unit,
                    UnitPrice = d.UnitPrice,
                    BorrowedQuantity = d.BorrowedQuantity,
                    UsedBorrowedQuantity = d.UsedBorrowedQuantity,
                    Total = d.Total,
                    Available = stocks.TryGetValue(d.ProductId, out var available) ? available : 0,
                    SerialNumbers = d.ProductItems.Select(x => x.SerialNumber).ToList()
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
            var normalizedPageIndex = pageIndex <= 0 ? 1 : pageIndex;
            var normalizedPageSize = pageSize <= 0 ? 10 : Math.Min(pageSize, 200);

            var baseQuery = _context.Orders
                .AsNoTracking()
                .Where(x => x.StoreId == storeId && !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var pattern = $"%{keyword.Trim()}%";
                baseQuery = baseQuery.Where(x =>
                    EF.Functions.ILike(x.Code, pattern)
                    || EF.Functions.ILike(x.CustomerName, pattern)
                    || EF.Functions.ILike(x.CustomerPhone, pattern));
            }

            if (dateFrom.HasValue)
            {
                var fromUtc = NormalizeDateTimeForPostgres(dateFrom.Value);
                baseQuery = baseQuery.Where(x => x.OrderDate >= fromUtc);
            }

            if (dateTo.HasValue)
            {
                var toUtc = NormalizeDateTimeForPostgres(dateTo.Value);
                baseQuery = baseQuery.Where(x => x.OrderDate <= toUtc);
            }

            var statusIdList = statusIds?
                .Where(x => x != Guid.Empty)
                .Distinct()
                .ToList();

            if (statusIdList is { Count: > 0 })
                baseQuery = baseQuery.Where(x => statusIdList.Contains(x.StatusId));

            var totalCount = await baseQuery.CountAsync(cancellationToken);

            var results = await baseQuery
                .OrderByDescending(x => x.Code)
                .Skip((normalizedPageIndex - 1) * normalizedPageSize)
                .Take(normalizedPageSize)
                .Select(x => new OrderCoreDto
                {
                    Id = x.Id,
                    StoreId = x.StoreId,
                    Code = x.Code,
                    OrderDate = x.OrderDate,
                    CustomerName = x.CustomerName,
                    CustomerType = x.CustomerType,
                    CustomerPhone = x.CustomerPhone,
                    CustomerId = x.CustomerId,
                    DeliveryAddress = x.DeliveryAddress,
                    TotalAmount = x.TotalAmount,
                    TechnicianId = x.TechnicianId,
                    TechnicianName = x.Technician != null ? x.Technician.Name : null,
                    InstallationDate = x.InstallationDate,
                    TechnicianExportedAt = x.TechnicianExportedAt,
                    StatusId = x.StatusId,
                    Description = x.Description,
                    StatusName = x.Status.Name,
                    CreatedDate = x.CreatedDate,
                    UpdatedDate = x.UpdatedDate,
                    RowVersion = x.RowVersion,
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted
                })
                .ToListAsync(cancellationToken);

            await AttachTechniciansAsync(results, cancellationToken);

            return (results, totalCount);
        }

        private static DateTime NormalizeDateTimeForPostgres(DateTime value)
            => value.Kind switch
            {
                DateTimeKind.Utc => value,
                DateTimeKind.Local => value.ToUniversalTime(),
                _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
            };



        public async Task UpdateStatusAsync(UpdateOrderStatusDto statusOrder, CancellationToken cancellationToken)
        {
            // Lưu ý: ExecuteUpdateAsync sẽ áp dụng trực tiếp trên DB; handler hiện tại dùng phương thức SaveChangesAsync chung nên
            // nếu muốn dùng ExecuteUpdateAsync, phải hiểu hành vi transaction của EF. Ở đây vẫn giữ cho các thao tác do handler quản lý.
            var affectedRows = await _context.Orders
                .Where(r => r.StoreId == statusOrder.StoreId && r.Id == statusOrder.Id && !r.IsDeleted)
                .ExecuteUpdateAsync(r => r
                    .SetProperty(x => x.StatusId, statusOrder.StatusId)
                    .SetProperty(x => x.RowVersion, statusOrder.RowVersion)
                    .SetProperty(x => x.UpdatedBy, statusOrder.UpdatedBy)
                    .SetProperty(x => x.UpdatedDate, statusOrder.UpdatedDate), cancellationToken);

            if (affectedRows == 0)
                throw new InvalidOperationException("Không thể cập nhật trạng thái đơn hàng. Vui lòng tải lại dữ liệu.");
        }

        public async Task UpdateOrderAsync(
        OrderCoreDto orderDto,
        CancellationToken cancellationToken)
        {
            var trackedOrder = await _context.Orders
                .Include(o => o.Details)
                .FirstOrDefaultAsync(
                    o => o.Id == orderDto.Id,
                    cancellationToken);

            if (trackedOrder == null)
                throw new InvalidOperationException(
                    $"Order {orderDto.Id} không tồn tại.");

            trackedOrder.CustomerId = orderDto.CustomerId;
            trackedOrder.CustomerType = orderDto.CustomerType;
            trackedOrder.CustomerName = orderDto.CustomerName;
            trackedOrder.CustomerPhone = orderDto.CustomerPhone;
            trackedOrder.DeliveryAddress = orderDto.DeliveryAddress;

            trackedOrder.TechnicianId =
                orderDto.TechnicianIds.FirstOrDefault();

            trackedOrder.InstallationDate = orderDto.InstallationDate;
            trackedOrder.Description = orderDto.Description;
            trackedOrder.StatusId = orderDto.StatusId;

            trackedOrder.UpdatedBy = orderDto.UpdatedBy;
            trackedOrder.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> UpdateDescriptionAsync(
            Guid storeId,
            Guid id,
            Guid rowVersion,
            string? description,
            Guid updatedBy,
            CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            return await _context.Orders
                .Where(x => x.StoreId == storeId
                    && x.Id == id
                    && x.RowVersion == rowVersion
                    && !x.IsDeleted)
                .ExecuteUpdateAsync(x => x
                    .SetProperty(o => o.Description, description)
                    .SetProperty(o => o.RowVersion, Guid.NewGuid())
                    .SetProperty(o => o.UpdatedBy, updatedBy)
                    .SetProperty(o => o.UpdatedDate, now), cancellationToken);
        }

        public async Task ReplaceOrderTechniciansAsync(
            Guid orderId,
            IEnumerable<Guid> technicianIds,
            CancellationToken cancellationToken = default)
        {
            var desiredIds = technicianIds
                .Where(x => x != Guid.Empty)
                .Distinct()
                .ToList();

            var existing = await _context.OrderTechnicians
                .Where(x => x.OrderId == orderId)
                .ToListAsync(cancellationToken);

            var desiredSet = desiredIds.ToHashSet();
            var toRemove = existing
                .Where(x => !desiredSet.Contains(x.TechnicianId))
                .ToList();

            if (toRemove.Count > 0)
                _context.OrderTechnicians.RemoveRange(toRemove);

            var existingIds = existing
                .Select(x => x.TechnicianId)
                .ToHashSet();

            var now = DateTime.UtcNow;
            var toAdd = desiredIds
                .Where(x => !existingIds.Contains(x))
                .Select(x => new OrderTechnician
                {
                    Id = Guid.NewGuid(),
                    OrderId = orderId,
                    TechnicianId = x,
                    AssignedAt = now,
                    CreatedDate = now
                })
                .ToList();

            if (toAdd.Count > 0)
                await _context.OrderTechnicians.AddRangeAsync(toAdd, cancellationToken);
        }


    }
}
