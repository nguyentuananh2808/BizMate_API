using BizMate.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SqlKata.Execution;
using Dapper;
using Npgsql;
using System.Data;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Dto.CoreDto;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Common;
using System.Text.Json;

namespace BizMate.Infrastructure.Persistence.Repositories
{
    public class ImportReceiptRepository : IImportReceiptRepository
    {
        private readonly AppDbContext _context;

        public ImportReceiptRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task UpdateWithDetailsAsync(
      ImportReceipt receipt,
      IEnumerable<ImportReceiptDetail> details,
      CancellationToken cancellationToken = default)
        {
            await using var conn = (NpgsqlConnection)_context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync(cancellationToken);

            await using var cmd = new NpgsqlCommand("CALL sp_update_import_receipt(@p_receipt_id, @p_supplier_name, @p_delivery_address, @p_description, @p_row_version, @p_updated_by, @p_details)", conn);

            cmd.Parameters.AddWithValue("p_receipt_id", receipt.Id);
            cmd.Parameters.AddWithValue("p_supplier_name", receipt.SupplierName ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("p_delivery_address", receipt.DeliveryAddress ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("p_description", receipt.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("p_row_version", receipt.RowVersion);
            cmd.Parameters.AddWithValue("p_updated_by", receipt.UpdatedBy);

            var detailsJson = JsonSerializer.Serialize(details);
            cmd.Parameters.Add("p_details", NpgsqlTypes.NpgsqlDbType.Jsonb).Value = detailsJson;

            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }



        public async Task AddAsync(ImportReceipt receipt, CancellationToken cancellationToken = default)
        {
            await _context.ImportReceipts.AddAsync(receipt, cancellationToken);
        }

        public Task UpdateAsync(ImportReceipt receipt, CancellationToken cancellationToken = default)
        {
            _context.ImportReceipts.Update(receipt);
            return Task.CompletedTask;
        }




        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var receipt = await _context.ImportReceipts
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted, cancellationToken);
            if (receipt != null)
            {
                receipt.IsDeleted = true;
                receipt.DeletedAt = DateTime.UtcNow;
                _context.ImportReceipts.Update(receipt);
            }
        }

        public async Task<ImportReceipt?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.ImportReceipts
                .AsNoTracking()
                .Include(r => r.Details)
                .Include(r => r.Status)
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted, cancellationToken);
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
    Guid storeId, DateTime? dateFrom, DateTime? dateTo, IEnumerable<Guid>? statusIds, string? keyword, int pageIndex, int pageSize, QueryFactory queryFactory)
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
                baseQuery.Where("r.CreatedDate", ">=", dateFrom.Value);

            if (dateTo.HasValue)
                baseQuery.Where("r.CreatedDate", "<=", dateTo.Value);

            if (statusIds != null && statusIds.Any())
                baseQuery.WhereIn("s.Id", statusIds);

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
                    StatusId = row.StatusId,
                    IsDraft = row.IsDraft,
                    IsCancelled = row.IsCancelled,
                    Description = row.Description,
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
