using BizMate.Domain.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using SqlKata.Execution;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Dto.CoreDto;

namespace BizMate.Infrastructure.Persistence.Repositories
{
    public class ImportReceiptRepository : IImportReceiptRepository
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction _currentTransaction;

        public ImportReceiptRepository(AppDbContext context)
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

        public async Task AddAsync(ImportReceipt receipt, CancellationToken cancellationToken)
        {

            await _context.ImportReceipts.AddAsync(receipt);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(ImportReceipt receipt)
        {
            // Update Receipt
            _context.ImportReceipts.Update(receipt);

            // Update Details
            foreach (var detail in receipt.Details)
            {
                if (_context.Entry(detail).State == EntityState.Detached)
                {
                    _context.ImportReceiptDetails.Attach(detail);
                }

                _context.Entry(detail).State = EntityState.Modified;
            }

            // Save changes
            await _context.SaveChangesAsync();
        }



        public async Task DeleteAsync(Guid id)
        {
            var receipt = await _context.ImportReceipts.Include(r => r.Details).FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
            if (receipt != null)
            {
                receipt.IsDeleted = true;
                receipt.DeletedAt = DateTime.UtcNow;
                _context.ImportReceipts.Update(receipt);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ImportReceipt?> GetByIdAsync(Guid id)
        {
            return await _context.ImportReceipts
                .AsNoTracking()
                .Include(r => r.Details)
                .Include(r => r.Status)
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        }

        public async Task<List<ImportReceipt>> SearchReceipts(Guid storeId, int? type, string? keyword, QueryFactory queryFactory)
        {
            var query = queryFactory.Query("ImportReceipts as r")
                .Where("r.StoreId", storeId)
                .WhereFalse("r.IsDeleted");

            if (type.HasValue)
                query.Where("r.Type", type.Value);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var keywordLower = keyword.ToLower();
                query.Where(q =>
                    q.WhereRaw(@"LOWER(r.""Code"") LIKE ?", $"%{keywordLower}%")
                     .OrWhereRaw(@"LOWER(r.""SupplierName"") LIKE ?", $"%{keywordLower}%"));
            }

            var result = await query.GetAsync<ImportReceipt>();
            return result.ToList();
        }

        public async Task<(List<ImportReceipt> Receipts, int TotalCount)> SearchReceiptsWithPaging(
    Guid storeId, DateTime? dateFrom, DateTime? dateTo, string? statusCode, string? keyword, int pageIndex, int pageSize, QueryFactory queryFactory)
        {
            var baseQuery = queryFactory.Query("ImportReceipts as r")
                .LeftJoin("Statuses as s", "r.StatusId", "s.Id")
                .Where("r.StoreId", storeId)
                .WhereFalse("r.IsDeleted");


            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var keywordLower = keyword.ToLower();
                baseQuery.Where(q =>
                    q.WhereRaw(@"LOWER(r.""Code"") LIKE ?", $"%{keywordLower}%")
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
                var receipt = new ImportReceipt
                {
                    Id = row.Id,
                    StoreId = row.StoreId,
                    Code = row.Code,
                    SupplierName = row.SupplierName,
                    DeliveryAddress = row.DeliveryAddress,
                    TotalAmount = row.TotalAmount,
                    StatusId = row.StatusId,
                    IsDraft = row.IsDraft,
                    IsCancelled = row.IsCancelled,

                    Status = row.Status_Id != null
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

        public async Task UpdateStatusAsync(UpdateImportReceiptStatusDto statusImportReceipt, CancellationToken cancellationToken)
        {
            await _context.ImportReceipts
                .Where(r => r.StoreId == statusImportReceipt.StoreId && r.Id == statusImportReceipt.Id)
                .ExecuteUpdateAsync(r => r
                    .SetProperty(x => x.StatusId, statusImportReceipt.StatusId)
                    .SetProperty(x => x.RowVersion, statusImportReceipt.RowVersion)
                    .SetProperty(x => x.UpdatedBy, statusImportReceipt.UpdatedBy)
                    .SetProperty(x => x.UpdatedDate, statusImportReceipt.UpdatedDate), cancellationToken);
        }
    }
}
