using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SqlKata.Execution;

namespace BizMate.Infrastructure.Persistence.Repositories
{
    public class ExportReceiptRepository : IExportReceiptRepository
    {
        private readonly AppDbContext _context;

        public ExportReceiptRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(ExportReceipt receipt, CancellationToken cancellationToken)
        {

            await _context.ExportReceipts.AddAsync(receipt);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<ExportReceipt?> GetByIdAsync(Guid id)
        {
            return await _context.ExportReceipts
                .AsNoTracking()
                .Include(r => r.Details)
                .Include(r => r.Status)
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        }

        public async Task<(List<ExportReceipt> Receipts, int TotalCount)> SearchReceiptsWithPaging(
    Guid storeId, DateTime? dateFrom, DateTime? dateTo,  string? keyword, int pageIndex, int pageSize, QueryFactory queryFactory)
        {
            var baseQuery = queryFactory.Query("ExportReceipts as r")
                //.LeftJoin("Statuses as s", "r.StatusId", "s.Id")
                .Where("r.StoreId", storeId)
                .WhereFalse("r.IsDeleted");


            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var keywordLower = keyword.ToLower();
                baseQuery.Where(q =>
                    q.WhereRaw(@"LOWER(r.""Code"") LIKE ?", $"%{keywordLower}%")
                    .OrWhereRaw(@"LOWER(r.""CustomerPhone"") LIKE ?", $"%{keywordLower}%")
                     .OrWhereRaw(@"LOWER(r.""CustomerName"") LIKE ?", $"%{keywordLower}%"));
            }

            if (dateFrom.HasValue)
                baseQuery.Where("r.CreatedDate", ">=", dateFrom.Value);

            if (dateTo.HasValue)
                baseQuery.Where("r.CreatedDate", "<=", dateTo.Value);

            var totalCount = await baseQuery.Clone().CountAsync<int>();

            var rows = await baseQuery
                .Offset((pageIndex - 1) * pageSize)
                .Limit(pageSize)
                .GetAsync();

            var results = rows.Select(row =>
            {
                var receipt = new ExportReceipt
                {
                    Id = row.Id,
                    StoreId = row.StoreId,
                    Code = row.Code,
                    CustomerName = row.CustomerName,
                    CustomerPhone = row.CustomerPhone,
                    DeliveryAddress = row.DeliveryAddress,
                    TotalAmount = row.TotalAmount,
                    StatusId = row.StatusId,
                    IsDraft = row.IsDraft,
                    IsCancelled = row.IsCancelled,
                };
                return receipt;
            }).ToList();

            return (results, totalCount);
        }
    }
}
