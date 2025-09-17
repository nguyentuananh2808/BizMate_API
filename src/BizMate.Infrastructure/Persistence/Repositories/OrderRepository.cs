using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Domain.Entities;
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
        public async Task AddAsync(Order receipt, CancellationToken cancellationToken)
        {

            await _context.Orders.AddAsync(receipt);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Order receipt)
        {
            // Update Receipt
            _context.Orders.Update(receipt);

            // Update Details
            foreach (var detail in receipt.Details)
            {
                if (_context.Entry(detail).State == EntityState.Detached)
                {
                    _context.OrderDetails.Attach(detail);
                }

                _context.Entry(detail).State = EntityState.Modified;
            }

            // Save changes
            await _context.SaveChangesAsync();
        }



        public async Task DeleteAsync(Guid id)
        {
            var receipt = await _context.Orders.Include(r => r.Details).FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
            if (receipt != null)
            {
                receipt.IsDeleted = true;
                receipt.DeletedAt = DateTime.UtcNow;
                _context.Orders.Update(receipt);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Order?> GetByIdAsync(Guid id)
        {
            return await _context.Orders
                .AsNoTracking()
                .Include(r => r.Details)
                .Include(r => r.Status)
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        }

        public async Task<List<Order>> SearchReceipts(Guid storeId, int? type, string? keyword, QueryFactory queryFactory)
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
    Guid storeId, DateTime? dateFrom, DateTime? dateTo, IEnumerable<Guid>? statusIds, string? keyword, int pageIndex, int pageSize, QueryFactory queryFactory)
        {
            var baseQuery = queryFactory.Query("Orders as r")
                .LeftJoin("Statuses as s", "r.StatusId", "s.Id")
                .Where("r.StoreId", storeId)
                .WhereFalse("r.IsDeleted");


            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var keywordLower = keyword.ToLower();
                baseQuery.Where(q =>
                    q.WhereRaw(@"LOWER(r.""Code"") LIKE ?", $"%{keywordLower}%")
                     .OrWhereRaw(@"LOWER(r.""CustomerName"") LIKE ?", $"%{keywordLower}%")
                     .OrWhereRaw(@"LOWER(r.""CustomerPhone"") LIKE ?", $"%{keywordLower}%"));
            }

            if (dateFrom.HasValue)
                baseQuery.Where("r.OrderDate", ">=", dateFrom.Value);

            if (dateTo.HasValue)
                baseQuery.Where("r.OrderDate", "<=", dateTo.Value);

            if (statusIds != null && statusIds.Any())
                baseQuery.WhereIn("s.Id", statusIds);

            var totalCount = await baseQuery.Clone().CountAsync<int>();

            var rows = await baseQuery
                .Select("r.*", "s.Id as Status_Id", "s.Name as Status_Name", "s.Code as Status_Code")
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
                Description=row.Description,
                StatusName = row.Status_Name
            }).ToList();


            return (results, totalCount);
        }


        public async Task UpdateStatusAsync(UpdateOrderStatusDto statusOrder, CancellationToken cancellationToken)
        {
            await _context.Orders
                .Where(r => r.StoreId == statusOrder.StoreId && r.Id == statusOrder.Id && !r.IsDeleted)
                .ExecuteUpdateAsync(r => r
                    .SetProperty(x => x.StatusId, statusOrder.StatusId)
                    .SetProperty(x => x.RowVersion, statusOrder.RowVersion)
                    .SetProperty(x => x.UpdatedBy, statusOrder.UpdatedBy)
                    .SetProperty(x => x.UpdatedDate, statusOrder.UpdatedDate), cancellationToken);
        }

    }
}
