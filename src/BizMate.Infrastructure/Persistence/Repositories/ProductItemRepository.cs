using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BizMate.Infrastructure.Persistence.Repositories
{
    public class ProductItemRepository : IProductItemRepository
    {
        private readonly AppDbContext _context;

        public ProductItemRepository(AppDbContext context) => _context = context;

        public async Task<ProductItem?> GetBySerialNumberAsync(
            string serialNumber, CancellationToken ct = default)
            => await _context.ProductItems
                .Include(i => i.Product)
                .FirstOrDefaultAsync(i => i.SerialNumber == serialNumber && !i.IsDeleted, ct);

        public async Task<List<ProductItem>> GetBySerialNumbersAsync(
            IEnumerable<string> serialNumbers,
            CancellationToken ct = default)
        {
            var serialList = serialNumbers
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (serialList.Count == 0)
                return new List<ProductItem>();

            return await _context.ProductItems
                .Include(i => i.Product)
                .Where(i => serialList.Contains(i.SerialNumber) && !i.IsDeleted)
                .ToListAsync(ct);
        }

        public async Task<ProductItem?> GetByIdWithTransactionsAsync(
            Guid id, CancellationToken ct = default)
            => await _context.ProductItems
                .Include(i => i.Product)
                .Include(i => i.Transactions.OrderBy(t => t.CreatedDate))
                .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted, ct);

        public async Task<(List<ProductItem> Items, int TotalCount)> GetByProductAsync(
            Guid storeId,
            Guid productId,
            ProductItemStatus? status,
            string? keyword,
            int pageIndex,
            int pageSize,
            CancellationToken ct = default)
        {
            var query = _context.ProductItems
                .Include(i => i.Product)
                .Where(i => i.StoreId == storeId
                         && i.ProductId == productId
                         && !i.IsDeleted);

            if (status.HasValue)
                query = query.Where(i => i.Status == status.Value);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var normalizedKeyword = keyword.Trim();
                query = query.Where(i => EF.Functions.ILike(i.SerialNumber, $"%{normalizedKeyword}%"));
            }

            var total = await query.CountAsync(ct);

            var items = await query
                .OrderByDescending(i => i.CreatedDate)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, total);
        }

        public async Task<int> CountInStockAsync(
            Guid storeId,
            Guid productId,
            CancellationToken ct = default)
            => await _context.ProductItems.CountAsync(
                i => i.StoreId == storeId
                  && i.ProductId == productId
                  && i.Status == ProductItemStatus.InStock
                  && !i.IsDeleted, ct);

        public async Task<List<ProductItem>> GetInStockItemsAsync(
            Guid storeId,
            Guid productId,
            CancellationToken ct = default)
            => await _context.ProductItems
                .Include(i => i.Product)
                .Where(i => i.StoreId == storeId
                         && i.ProductId == productId
                         && i.Status == ProductItemStatus.InStock
                         && !i.IsDeleted)
                .OrderBy(i => i.CreatedDate)
                .ToListAsync(ct);

        public async Task<List<ProductItem>> GetByImportReceiptDetailIdsAsync(
            IEnumerable<Guid> importReceiptDetailIds,
            ProductItemStatus? status,
            CancellationToken ct = default)
        {
            var detailIds = importReceiptDetailIds.Distinct().ToList();
            if (detailIds.Count == 0)
                return new List<ProductItem>();

            var query = _context.ProductItems
                .Include(i => i.Product)
                .Where(i => i.ImportReceiptDetailId.HasValue
                         && detailIds.Contains(i.ImportReceiptDetailId.Value)
                         && !i.IsDeleted);

            if (status.HasValue)
                query = query.Where(i => i.Status == status.Value);

            return await query.ToListAsync(ct);
        }

        public async Task<List<ProductItem>> GetByOrderDetailIdsAsync(
            IEnumerable<Guid> orderDetailIds,
            ProductItemStatus? status,
            CancellationToken ct = default)
        {
            var detailIds = orderDetailIds.Distinct().ToList();
            if (detailIds.Count == 0)
                return new List<ProductItem>();

            var query = _context.ProductItems
                .Include(i => i.Product)
                .Where(i => i.OrderDetailId.HasValue
                         && detailIds.Contains(i.OrderDetailId.Value)
                         && !i.IsDeleted);

            if (status.HasValue)
                query = query.Where(i => i.Status == status.Value);

            return await query.ToListAsync(ct);
        }

        public async Task<bool> ExistsBySerialNumberAsync(
            string serialNumber,
            CancellationToken ct = default)
            => await _context.ProductItems.AnyAsync(
                i => i.SerialNumber == serialNumber && !i.IsDeleted, ct);

        public async Task AddAsync(ProductItem item, CancellationToken ct = default)
            => await _context.ProductItems.AddAsync(item, ct);

        public async Task AddRangeAsync(
            IEnumerable<ProductItem> items,
            CancellationToken ct = default)
            => await _context.ProductItems.AddRangeAsync(items, ct);

        public Task UpdateAsync(ProductItem item, CancellationToken ct = default)
        {
            _context.ProductItems.Update(item);
            return Task.CompletedTask;
        }

        public Task UpdateRangeAsync(IEnumerable<ProductItem> items, CancellationToken ct = default)
        {
            _context.ProductItems.UpdateRange(items);
            return Task.CompletedTask;
        }
    }
}
