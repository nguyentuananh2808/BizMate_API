using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using SqlKata.Execution;

namespace BizMate.Infrastructure.Persistence.Repositories
{
    public class InventoryReceiptRepository : IInventoryReceiptRepository
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction _currentTransaction;

        public InventoryReceiptRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task BeginTransactionAsync()
        {
            _currentTransaction = await _context.Database.BeginTransactionAsync();
        }
        public EntityEntry Entry(object entity)
        {
            return _context.Entry(entity);
        }

        public async Task CommitTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.CommitAsync();
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                try
                {
                    await _currentTransaction.RollbackAsync();
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine("Transaction already completed. Skip rollback.");
                }
                finally
                {
                    await _currentTransaction.DisposeAsync();
                    _currentTransaction = null;
                }
            }
        }

        public async Task AddAsync(InventoryReceipt receipt)
        {

            await _context.InventoryReceipts.AddAsync(receipt);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(InventoryReceipt receipt)
        {
            // Attach entity nếu chưa được gắn vào context
            var entry = _context.Entry(receipt);
            if (entry.State == EntityState.Detached)
            {
                _context.InventoryReceipts.Attach(receipt);
            }

            // Chỉ đánh dấu các thuộc tính chính là Modified (không động đến RowVersion)
            entry.Property(x => x.SupplierName).IsModified = true;
            entry.Property(x => x.CustomerName).IsModified = true;
            entry.Property(x => x.CustomerPhone).IsModified = true;
            entry.Property(x => x.DeliveryAddress).IsModified = true;
            entry.Property(x => x.Description).IsModified = true;
            entry.Property(x => x.UpdatedDate).IsModified = true;

            // ⚠️ Gán OriginalValue cho RowVersion để EF kiểm tra concurrency
            entry.Property(x => x.RowVersion).OriginalValue = receipt.RowVersion;

            await _context.SaveChangesAsync();
        }


        public async Task DeleteAsync(Guid id)
        {
            var receipt = await _context.InventoryReceipts.Include(r => r.Details).FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
            if (receipt != null)
            {
                receipt.IsDeleted = true;
                receipt.DeletedAt = DateTime.UtcNow;
                _context.InventoryReceipts.Update(receipt);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<InventoryReceipt?> GetByIdAsync(Guid id)
        {
            return await _context.InventoryReceipts
                .Include(r => r.Details)
                .Include(r => r.Statuses)
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        }

        public async Task<List<InventoryReceipt>> SearchReceipts(Guid storeId, int? type, string? keyword, QueryFactory queryFactory)
        {
            var query = queryFactory.Query("InventoryReceipts as r")
                .Where("r.StoreId", storeId)
                .WhereFalse("r.IsDeleted");

            if (type.HasValue)
                query.Where("r.Type", type.Value);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var keywordLower = keyword.ToLower();
                query.Where(q =>
                    q.WhereRaw(@"LOWER(r.""InventoryCode"") LIKE ?", $"%{keywordLower}%")
                     .OrWhereRaw(@"LOWER(r.""CustomerName"") LIKE ?", $"%{keywordLower}%")
                     .OrWhereRaw(@"LOWER(r.""SupplierName"") LIKE ?", $"%{keywordLower}%"));
            }

            var result = await query.GetAsync<InventoryReceipt>();
            return result.ToList();
        }

        public async Task<(List<InventoryReceipt> Receipts, int TotalCount)> SearchReceiptsWithPaging(
    Guid storeId, DateTime? dateFrom, DateTime? dateTo, string? statusCode, int? type, string? keyword, int pageIndex, int pageSize, QueryFactory queryFactory)
        {
            var baseQuery = queryFactory.Query("InventoryReceipts as r")
                .LeftJoin("Statuses as s", "r.StatusId", "s.Id")
                .Where("r.StoreId", storeId)
                .WhereFalse("r.IsDeleted");

            if (type.HasValue)
                baseQuery.Where("r.Type", type.Value);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var keywordLower = keyword.ToLower();
                baseQuery.Where(q =>
                    q.WhereRaw(@"LOWER(r.""InventoryCode"") LIKE ?", $"%{keywordLower}%")
                     .OrWhereRaw(@"LOWER(r.""CustomerName"") LIKE ?", $"%{keywordLower}%")
                     .OrWhereRaw(@"LOWER(r.""SupplierName"") LIKE ?", $"%{keywordLower}%"));
            }

            if (dateFrom.HasValue)
                baseQuery.Where("r.Date", ">=", dateFrom.Value);

            if (dateTo.HasValue)
                baseQuery.Where("r.Date", "<=", dateTo.Value);

            if (!string.IsNullOrWhiteSpace(statusCode))
                baseQuery.Where("s.Code", statusCode);

            var totalCount = await baseQuery.Clone().CountAsync<int>();

            var rows = await baseQuery
                .Select("r.*", "s.Id as Status_Id", "s.Name as Status_Name", "s.Code as Status_Code")
                .Offset((pageIndex - 1) * pageSize)
                .Limit(pageSize)
                .GetAsync();

            var results = rows.Select(row =>
            {
                var receipt = new InventoryReceipt
                {
                    Id = row.Id,
                    StoreId = row.StoreId,
                    Type = row.Type,
                    Date = row.Date,
                    Code = row.Code,
                    CustomerName = row.CustomerName,
                    SupplierName = row.SupplierName,
                    CustomerPhone = row.CustomerPhone,
                    DeliveryAddress = row.DeliveryAddress,
                    TotalAmount = row.TotalAmount,
                    StatusId = row.StatusId,
                    IsDraft = row.IsDraft,
                    IsCancelled = row.IsCancelled,

                    Statuses = row.Status_Id != null
                        ? new Status
                        {
                            Id = row.Status_Id,
                            Code = row.Status_Code,
                            Name = row.Status_Name
                        }
                        : null
                };
                return receipt;
            }).ToList();

            return (results, totalCount);
        }



    }
}
