using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Domain.Entities;
using BizMate.Infrastructure.Migrations;
using Microsoft.EntityFrameworkCore;

namespace BizMate.Infrastructure.Persistence.Repositories
{
    public class TechnicianHoldingRepository : ITechnicianHoldingRepository
    {
        private readonly AppDbContext _context;

        public TechnicianHoldingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Order?> GetOrderWithDetailsAsync(Guid orderId, Guid storeId, CancellationToken ct = default)
        {
            return await _context.Orders
                .Include(x => x.Details)
                .Include(x => x.Status)
                .Include(x => x.Technician)
                .Include(x => x.OrderTechnicians)
                    .ThenInclude(x => x.Technician)
                .FirstOrDefaultAsync(x => x.Id == orderId
                    && x.StoreId == storeId
                    && !x.IsDeleted, ct);
        }

        public async Task<List<Technician>> SearchTechniciansAsync(
            Guid storeId,
            string? keyword,
            bool includeInactive = false,
            CancellationToken ct = default)
        {
            var query = _context.Technicians
                .Where(x => x.StoreId == storeId && !x.IsDeleted);

            if (!includeInactive)
                query = query.Where(x => x.IsActive);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var normalized = keyword.Trim().ToLower();
                query = query.Where(x =>
                    x.Name.ToLower().Contains(normalized)
                    || (x.Phone != null && x.Phone.ToLower().Contains(normalized))
                    || (x.ZaloPhone != null && x.ZaloPhone.ToLower().Contains(normalized)));
            }

            return await query
                .OrderBy(x => x.Name)
                .ToListAsync(ct);
        }

        public async Task<Technician?> GetTechnicianAsync(Guid technicianId, Guid storeId, CancellationToken ct = default)
        {
            return await _context.Technicians
                .FirstOrDefaultAsync(x => x.Id == technicianId
                    && x.StoreId == storeId
                    && !x.IsDeleted, ct);
        }

        public async Task<Technician?> GetTechnicianByUserIdAsync(
            Guid userId,
            Guid storeId,
            CancellationToken ct = default)
        {
            return await _context.Technicians
                .FirstOrDefaultAsync(x => x.UserId == userId
                    && x.StoreId == storeId
                    && !x.IsDeleted, ct);
        }

        public async Task<List<Technician>> GetTechniciansByIdsAsync(
            Guid storeId,
            IEnumerable<Guid> technicianIds,
            CancellationToken ct = default)
        {
            var ids = technicianIds.Where(x => x != Guid.Empty).Distinct().ToList();
            if (ids.Count == 0)
                return new List<Technician>();

            return await _context.Technicians
                .Where(x => x.StoreId == storeId
                    && ids.Contains(x.Id)
                    && !x.IsDeleted)
                .ToListAsync(ct);
        }

        public async Task<bool> ExistsTechnicianPhoneAsync(
            Guid storeId,
            string phone,
            Guid? excludeId = null,
            CancellationToken ct = default)
        {
            var normalized = phone.Trim().ToLower();
            return await _context.Technicians.AnyAsync(x =>
                x.StoreId == storeId
                && !x.IsDeleted
                && x.Phone != null
                && x.Phone.ToLower() == normalized
                && (!excludeId.HasValue || x.Id != excludeId.Value), ct);
        }

        public async Task<List<Stock>> GetStocksAsync(Guid storeId, IEnumerable<Guid> productIds, CancellationToken ct = default)
        {
            var ids = productIds.Distinct().ToList();
            return await _context.Stocks
                .Include(x => x.Product)
                .Where(x => x.StoreId == storeId
                    && ids.Contains(x.ProductId)
                    && !x.IsDeleted)
                .ToListAsync(ct);
        }

        public async Task<TechnicianHolding?> GetHoldingAsync(
            Guid storeId,
            Guid technicianId,
            Guid productId,
            CancellationToken ct = default)
        {
            return await _context.TechnicianHoldings
                .Include(x => x.Product)
                .Include(x => x.Technician)
                .FirstOrDefaultAsync(x => x.StoreId == storeId
                    && x.TechnicianId == technicianId
                    && x.ProductId == productId
                    && x.BorrowType == TechnicianBorrowType.Assigned
                    && !x.IsDeleted, ct);
        }

        public async Task<TechnicianHolding?> GetHoldingByTypeAsync(
            Guid storeId,
            Guid technicianId,
            Guid productId,
            TechnicianBorrowType borrowType,
            CancellationToken ct = default)
        {
            return await _context.TechnicianHoldings
                .Include(x => x.Product)
                .Include(x => x.Technician)
                .FirstOrDefaultAsync(x => x.StoreId == storeId
                    && x.TechnicianId == technicianId
                    && x.ProductId == productId
                    && x.BorrowType == borrowType
                    && !x.IsDeleted, ct);
        }

        public async Task<List<TechnicianHolding>> GetHoldingsByProductAsync(
            Guid storeId,
            Guid productId,
            CancellationToken ct = default)
        {
            return await _context.TechnicianHoldings
                .Include(x => x.Product)
                .Include(x => x.Technician)
                .Where(x => x.StoreId == storeId
                    && x.ProductId == productId
                    && x.Quantity > 0
                    && !x.IsDeleted)
                .ToListAsync(ct);
        }

        public async Task<List<TechnicianHolding>> GetHoldingsAsync(
            Guid storeId,
            Guid? technicianId = null,
            CancellationToken ct = default)
        {
            var query = _context.TechnicianHoldings
                .Include(x => x.Product)
                .Include(x => x.Technician)
                .Where(x => x.StoreId == storeId
                    && x.Quantity > 0
                    && !x.IsDeleted);

            if (technicianId.HasValue)
                query = query.Where(x => x.TechnicianId == technicianId.Value);

            return await query
                .OrderBy(x => x.Technician.Name)
                .ThenBy(x => x.Product.Name)
                .ToListAsync(ct);
        }

        public async Task<List<TechnicianHolding>> GetOverdueHoldingsAsync(
            Guid storeId,
            DateTime overdueBefore,
            CancellationToken ct = default)
        {
            return await _context.TechnicianHoldings
                .Include(x => x.Product)
                .Include(x => x.Technician)
                .Where(x => x.StoreId == storeId
                    && x.BorrowType == TechnicianBorrowType.Daily
                    && x.Quantity > 0
                    && x.LastBorrowedAt <= overdueBefore
                    && !x.IsDeleted)
                .OrderBy(x => x.LastBorrowedAt)
                .ToListAsync(ct);
        }

        public async Task<List<TechnicianBorrowRequest>> GetBorrowRequestsAsync(
            Guid storeId,
            TechnicianBorrowRequestStatus? status,
            Guid? technicianId = null,
            CancellationToken ct = default)
        {
            var query = _context.TechnicianBorrowRequests
                .Include(x => x.Technician)
                .Include(x => x.Items)
                    .ThenInclude(x => x.Product)
                .Where(x => x.StoreId == storeId && !x.IsDeleted);

            if (status.HasValue)
                query = query.Where(x => x.RequestStatus == status.Value);

            if (technicianId.HasValue)
                query = query.Where(x => x.TechnicianId == technicianId.Value);

            return await query
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync(ct);
        }

        public async Task<TechnicianBorrowRequest?> GetBorrowRequestAsync(
            Guid storeId,
            Guid requestId,
            CancellationToken ct = default)
        {
            return await _context.TechnicianBorrowRequests
                .Include(x => x.Technician)
                .Include(x => x.Items)
                    .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.StoreId == storeId
                    && x.Id == requestId
                    && !x.IsDeleted, ct);
        }

        public async Task<List<SalesByProductReportRow>> GetSalesByProductAsync(
            Guid storeId,
            DateTime? dateFrom,
            DateTime? dateTo,
            CancellationToken ct = default)
        {
            var query = _context.OrderDetails
                .Include(x => x.Order)
                .Where(x => x.Order.StoreId == storeId
                    && !x.IsDeleted
                    && !x.Order.IsDeleted);

            if (dateFrom.HasValue)
                query = query.Where(x => x.Order.OrderDate >= dateFrom.Value);

            if (dateTo.HasValue)
                query = query.Where(x => x.Order.OrderDate <= dateTo.Value);

            return await query
                .GroupBy(x => new { x.ProductId, x.ProductName, x.ProductCode })
                .Select(g => new SalesByProductReportRow
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    ProductCode = g.Key.ProductCode,
                    OrderedQuantity = g.Sum(x => x.Quantity),
                    UsedBorrowedQuantity = g.Sum(x => x.UsedBorrowedQuantity),
                    TotalSoldQuantity = g.Sum(x => x.Quantity + x.UsedBorrowedQuantity)
                })
                .OrderByDescending(x => x.TotalSoldQuantity)
                .ToListAsync(ct);
        }

        public async Task<int> DecreaseStockAsync(
            Guid stockId,
            int quantity,
            int reservedToRelease,
            Guid? userId,
            DateTime now,
            CancellationToken ct = default)
        {
            var rowVersion = Guid.NewGuid();

            return await _context.Stocks
                .Where(x => x.Id == stockId
                    && !x.IsDeleted
                    && x.Quantity >= quantity
                    && x.Reserved >= reservedToRelease
                    && x.Quantity - x.Reserved >= quantity - reservedToRelease)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.Quantity, x => x.Quantity - quantity)
                    .SetProperty(x => x.Reserved, x => x.Reserved - reservedToRelease)
                    .SetProperty(x => x.UpdatedBy, userId)
                    .SetProperty(x => x.UpdatedDate, now)
                    .SetProperty(x => x.RowVersion, rowVersion), ct);
        }

        public async Task<int> IncreaseOrderDetailBorrowedQuantityAsync(
            Guid orderDetailId,
            int quantity,
            Guid? userId,
            DateTime now,
            CancellationToken ct = default)
        {
            var rowVersion = Guid.NewGuid();

            return await _context.OrderDetails
                .Where(x => x.Id == orderDetailId && !x.IsDeleted)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.BorrowedQuantity, x => x.BorrowedQuantity + quantity)
                    .SetProperty(x => x.UpdatedBy, userId)
                    .SetProperty(x => x.UpdatedDate, now)
                    .SetProperty(x => x.RowVersion, rowVersion), ct);
        }

        public async Task<int> IncreaseHoldingQuantityAsync(
            Guid holdingId,
            int quantity,
            Guid? userId,
            DateTime now,
            CancellationToken ct = default)
        {
            var rowVersion = Guid.NewGuid();

            return await _context.TechnicianHoldings
                .Where(x => x.Id == holdingId && !x.IsDeleted)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.Quantity, x => x.Quantity + quantity)
                    .SetProperty(x => x.LastBorrowedAt, now)
                    .SetProperty(x => x.UpdatedBy, userId)
                    .SetProperty(x => x.UpdatedDate, now)
                    .SetProperty(x => x.RowVersion, rowVersion), ct);
        }

        public void AddOrderDetail(OrderDetail detail)
        {
            _context.OrderDetails.Add(detail);
        }

        public void AddHolding(TechnicianHolding holding)
        {
            _context.TechnicianHoldings.Add(holding);
        }

        public void AddBorrowRequest(TechnicianBorrowRequest request)
        {
            _context.TechnicianBorrowRequests.Add(request);
        }

        public void AddTechnician(Technician technician)
        {
            _context.Technicians.Add(technician);
        }

        public void AddTransaction(HoldingTransaction transaction)
        {
            _context.HoldingTransactions.Add(transaction);
        }
    }
}
