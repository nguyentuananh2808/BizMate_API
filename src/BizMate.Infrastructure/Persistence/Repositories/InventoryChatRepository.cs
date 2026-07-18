using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.UserCases.InventoryChat;
using BizMate.Domain.Entities;
using BizMate.Infrastructure.Migrations;
using Microsoft.EntityFrameworkCore;

namespace BizMate.Infrastructure.Persistence.Repositories
{
    public class InventoryChatRepository : IInventoryChatRepository
    {
        private readonly AppDbContext _context;

        public InventoryChatRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<InventoryChatProductStockDto>> SearchProductsAsync(
            Guid storeId,
            string? keyword,
            int limit,
            CancellationToken cancellationToken)
        {
            var rows = await GetProductStockRowsAsync(storeId, cancellationToken);
            return ApplyKeyword(rows, keyword)
                .OrderByDescending(x => x.Available > 0)
                .ThenBy(x => x.Name)
                .Take(limit)
                .ToArray();
        }

        public async Task<IReadOnlyList<InventoryChatProductStockDto>> GetLowStockProductsAsync(
            Guid storeId,
            int threshold,
            int limit,
            CancellationToken cancellationToken)
        {
            var rows = await GetProductStockRowsAsync(storeId, cancellationToken);
            return rows
                .Where(x => x.Available < threshold)
                .OrderBy(x => x.Available)
                .ThenBy(x => x.Name)
                .Take(limit)
                .ToArray();
        }

        public async Task<IReadOnlyList<InventoryChatProductStockDto>> GetReservedStockProductsAsync(
            Guid storeId,
            string? keyword,
            int limit,
            CancellationToken cancellationToken)
        {
            var rows = await GetProductStockRowsAsync(storeId, cancellationToken);
            return ApplyKeyword(rows.Where(x => x.Reserved > 0), keyword)
                .OrderByDescending(x => x.Reserved)
                .ThenBy(x => x.Name)
                .Take(limit)
                .ToArray();
        }

        public async Task<IReadOnlyList<InventoryChatProductStockDto>> GetSerialTrackedProductsAsync(
            Guid storeId,
            string? keyword,
            int limit,
            CancellationToken cancellationToken)
        {
            var rows = await GetProductStockRowsAsync(storeId, cancellationToken);
            return ApplyKeyword(rows.Where(x => x.IsSerialTracked), keyword)
                .OrderByDescending(x => x.Available)
                .ThenBy(x => x.Name)
                .Take(limit)
                .ToArray();
        }

        public async Task<InventoryChatStockSummaryDto> GetStockSummaryAsync(
            Guid storeId,
            int lowStockThreshold,
            CancellationToken cancellationToken)
        {
            var rows = await GetProductStockRowsAsync(storeId, cancellationToken);
            return new InventoryChatStockSummaryDto
            {
                ProductCount = rows.Count,
                SerialTrackedCount = rows.Count(x => x.IsSerialTracked),
                TotalQuantity = rows.Sum(x => x.Quantity),
                TotalReserved = rows.Sum(x => x.Reserved),
                TotalAvailable = rows.Sum(x => x.Available),
                LowStockCount = rows.Count(x => x.Available > 0 && x.Available < lowStockThreshold),
                OutOfStockCount = rows.Count(x => x.Available <= 0)
            };
        }

        public async Task<IReadOnlyList<InventoryChatHoldingDto>> SearchTechnicianHoldingsAsync(
            Guid storeId,
            string? keyword,
            int limit,
            CancellationToken cancellationToken)
        {
            var rows = await _context.TechnicianHoldings
                .AsNoTracking()
                .Include(x => x.Technician)
                .Include(x => x.Product)
                .Where(x => x.StoreId == storeId
                    && x.Quantity > 0
                    && !x.IsDeleted)
                .Select(x => new InventoryChatHoldingDto
                {
                    TechnicianName = x.Technician.Name,
                    TechnicianPhone = x.Technician.Phone ?? x.Technician.ZaloPhone,
                    ProductCode = x.Product.Code,
                    ProductName = x.Product.Name,
                    BorrowType = x.BorrowType == TechnicianBorrowType.Daily
                        ? "Mượn trong ngày"
                        : "Giữ trong balo",
                    Quantity = x.Quantity,
                    LastBorrowedAt = x.LastBorrowedAt
                })
                .ToListAsync(cancellationToken);

            return ApplyKeyword(rows, keyword, x =>
                    $"{x.TechnicianName} {x.TechnicianPhone} {x.ProductCode} {x.ProductName} {x.BorrowType}")
                .OrderBy(x => x.TechnicianName)
                .ThenBy(x => x.ProductName)
                .Take(limit)
                .ToArray();
        }

        public async Task<IReadOnlyList<InventoryChatReceiptDto>> GetImportReceiptsAsync(
            Guid storeId,
            DateTime fromUtc,
            DateTime toUtc,
            string? keyword,
            int limit,
            CancellationToken cancellationToken)
        {
            var receipts = await _context.ImportReceipts
                .AsNoTracking()
                .Include(x => x.Details)
                .Where(x => x.StoreId == storeId
                    && x.CreatedDate >= fromUtc
                    && x.CreatedDate < toUtc
                    && !x.IsDeleted)
                .OrderByDescending(x => x.CreatedDate)
                .Take(100)
                .ToListAsync(cancellationToken);

            return ApplyReceiptKeyword(receipts, keyword)
                .Select(x => new InventoryChatReceiptDto
                {
                    Code = x.Code,
                    CreatedDate = x.CreatedDate,
                    PartnerName = x.SupplierName,
                    TotalAmount = x.TotalAmount,
                    TotalQuantity = x.Details.Where(d => !d.IsDeleted).Sum(d => d.Quantity),
                    ProductsSummary = BuildProductsSummary(x.Details)
                })
                .Take(limit)
                .ToArray();
        }

        public async Task<IReadOnlyList<InventoryChatReceiptDto>> GetExportReceiptsAsync(
            Guid storeId,
            DateTime fromUtc,
            DateTime toUtc,
            string? keyword,
            int limit,
            CancellationToken cancellationToken)
        {
            var receipts = await _context.ExportReceipts
                .AsNoTracking()
                .Include(x => x.Details)
                .Where(x => x.StoreId == storeId
                    && x.CreatedDate >= fromUtc
                    && x.CreatedDate < toUtc
                    && !x.IsDeleted)
                .OrderByDescending(x => x.CreatedDate)
                .Take(100)
                .ToListAsync(cancellationToken);

            return ApplyReceiptKeyword(receipts, keyword)
                .Select(x => new InventoryChatReceiptDto
                {
                    Code = x.Code,
                    CreatedDate = x.CreatedDate,
                    PartnerName = x.CustomerName,
                    TotalAmount = x.TotalAmount,
                    TotalQuantity = x.Details.Where(d => !d.IsDeleted).Sum(d => d.Quantity),
                    ProductsSummary = BuildProductsSummary(x.Details)
                })
                .Take(limit)
                .ToArray();
        }

        public async Task<IReadOnlyList<InventoryChatHistoryDto>> GetProductHistoryAsync(
            Guid storeId,
            string productKeyword,
            DateTime? fromUtc,
            DateTime? toUtc,
            int limit,
            CancellationToken cancellationToken)
        {
            var products = await SearchProductsAsync(storeId, productKeyword, 20, cancellationToken);
            var productIds = products.Select(x => x.ProductId).Distinct().ToArray();
            if (productIds.Length == 0)
                return Array.Empty<InventoryChatHistoryDto>();

            var history = new List<InventoryChatHistoryDto>();
            history.AddRange(await GetImportHistory(storeId, productIds, fromUtc, toUtc, cancellationToken));
            history.AddRange(await GetExportHistory(storeId, productIds, fromUtc, toUtc, cancellationToken));
            history.AddRange(await GetInventoryTransactionHistory(storeId, productIds, fromUtc, toUtc, cancellationToken));
            history.AddRange(await GetHoldingTransactionHistory(storeId, productIds, fromUtc, toUtc, cancellationToken));

            return history
                .OrderByDescending(x => x.Date)
                .Take(limit)
                .ToArray();
        }

        private async Task<List<InventoryChatProductStockDto>> GetProductStockRowsAsync(
            Guid storeId,
            CancellationToken cancellationToken)
        {
            return await (
                from product in _context.Products.AsNoTracking()
                join stockValue in _context.Stocks.AsNoTracking()
                        .Where(x => x.StoreId == storeId && !x.IsDeleted)
                    on product.Id equals stockValue.ProductId into stockJoin
                from stock in stockJoin.DefaultIfEmpty()
                join categoryValue in _context.ProductCategories.AsNoTracking()
                        .Where(x => x.StoreId == storeId && !x.IsDeleted)
                    on product.ProductCategoryId equals categoryValue.Id into categoryJoin
                from category in categoryJoin.DefaultIfEmpty()
                where product.StoreId == storeId
                    && !product.IsDeleted
                    && !product.IsActive
                select new InventoryChatProductStockDto
                {
                    ProductId = product.Id,
                    Code = product.Code,
                    Name = product.Name,
                    CategoryName = category != null ? category.Name : null,
                    Quantity = stock != null ? stock.Quantity : 0,
                    Reserved = stock != null ? stock.Reserved : 0,
                    Available = stock != null ? stock.Quantity - stock.Reserved : 0,
                    IsSerialTracked = product.IsSerialTracked
                })
                .ToListAsync(cancellationToken);
        }

        private async Task<IReadOnlyList<InventoryChatHistoryDto>> GetImportHistory(
            Guid storeId,
            Guid[] productIds,
            DateTime? fromUtc,
            DateTime? toUtc,
            CancellationToken cancellationToken)
        {
            var query = _context.ImportReceiptDetails
                .AsNoTracking()
                .Include(x => x.ImportReceipt)
                .Where(x => productIds.Contains(x.ProductId)
                    && x.ImportReceipt.StoreId == storeId
                    && !x.IsDeleted
                    && !x.ImportReceipt.IsDeleted);

            query = ApplyDateRange(query, x => x.ImportReceipt.CreatedDate, fromUtc, toUtc);

            return await query.Select(x => new InventoryChatHistoryDto
                {
                    Date = x.ImportReceipt.CreatedDate,
                    Source = "Nhập kho",
                    ReferenceCode = x.ImportReceipt.Code,
                    ProductCode = x.ProductCode ?? string.Empty,
                    ProductName = x.ProductName,
                    Quantity = x.Quantity,
                    Actor = x.ImportReceipt.SupplierName,
                    Note = x.ImportReceipt.Description
                })
                .ToListAsync(cancellationToken);
        }

        private async Task<IReadOnlyList<InventoryChatHistoryDto>> GetExportHistory(
            Guid storeId,
            Guid[] productIds,
            DateTime? fromUtc,
            DateTime? toUtc,
            CancellationToken cancellationToken)
        {
            var query = _context.ExportReceiptDetails
                .AsNoTracking()
                .Include(x => x.ExportReceipt)
                .Where(x => productIds.Contains(x.ProductId)
                    && x.ExportReceipt.StoreId == storeId
                    && !x.IsDeleted
                    && !x.ExportReceipt.IsDeleted);

            query = ApplyDateRange(query, x => x.ExportReceipt.CreatedDate, fromUtc, toUtc);

            return await query.Select(x => new InventoryChatHistoryDto
                {
                    Date = x.ExportReceipt.CreatedDate,
                    Source = "Xuất kho",
                    ReferenceCode = x.ExportReceipt.Code,
                    ProductCode = x.ProductCode ?? string.Empty,
                    ProductName = x.ProductName,
                    Quantity = x.Quantity,
                    Actor = x.ExportReceipt.CustomerName,
                    Note = x.ExportReceipt.Description
                })
                .ToListAsync(cancellationToken);
        }

        private async Task<IReadOnlyList<InventoryChatHistoryDto>> GetInventoryTransactionHistory(
            Guid storeId,
            Guid[] productIds,
            DateTime? fromUtc,
            DateTime? toUtc,
            CancellationToken cancellationToken)
        {
            var query = _context.InventoryTransactions
                .AsNoTracking()
                .Include(x => x.ProductItem)
                    .ThenInclude(x => x.Product)
                .Where(x => productIds.Contains(x.ProductItem.ProductId)
                    && x.ProductItem.StoreId == storeId
                    && !x.IsDeleted
                    && !x.ProductItem.IsDeleted);

            query = ApplyDateRange(query, x => x.CreatedDate, fromUtc, toUtc);

            var rows = await query.Select(x => new
                {
                    x.CreatedDate,
                    x.Type,
                    x.ProductItem.SerialNumber,
                    ProductCode = x.ProductItem.Product.Code,
                    ProductName = x.ProductItem.Product.Name,
                    x.Note
                })
                .ToListAsync(cancellationToken);

            return rows.Select(x => new InventoryChatHistoryDto
            {
                Date = x.CreatedDate,
                Source = MapInventoryTransactionType(x.Type),
                ReferenceCode = x.SerialNumber,
                ProductCode = x.ProductCode,
                ProductName = x.ProductName,
                Quantity = 1,
                Actor = "Serial",
                Note = x.Note
            }).ToArray();
        }

        private async Task<IReadOnlyList<InventoryChatHistoryDto>> GetHoldingTransactionHistory(
            Guid storeId,
            Guid[] productIds,
            DateTime? fromUtc,
            DateTime? toUtc,
            CancellationToken cancellationToken)
        {
            var query = _context.HoldingTransactions
                .AsNoTracking()
                .Include(x => x.Product)
                .Include(x => x.Technician)
                .Where(x => x.StoreId == storeId
                    && productIds.Contains(x.ProductId)
                    && !x.IsDeleted);

            query = ApplyDateRange(query, x => x.CreatedDate, fromUtc, toUtc);

            var rows = await query.Select(x => new
                {
                    x.CreatedDate,
                    x.Type,
                    x.ReferenceType,
                    ProductCode = x.Product.Code,
                    ProductName = x.Product.Name,
                    x.Quantity,
                    TechnicianName = x.Technician.Name,
                    x.Note
                })
                .ToListAsync(cancellationToken);

            return rows.Select(x => new InventoryChatHistoryDto
            {
                Date = x.CreatedDate,
                Source = MapHoldingTransactionType(x.Type),
                ReferenceCode = x.ReferenceType,
                ProductCode = x.ProductCode,
                ProductName = x.ProductName,
                Quantity = x.Quantity,
                Actor = x.TechnicianName,
                Note = x.Note
            }).ToArray();
        }

        private static IQueryable<T> ApplyDateRange<T>(
            IQueryable<T> query,
            System.Linq.Expressions.Expression<Func<T, DateTime>> selector,
            DateTime? fromUtc,
            DateTime? toUtc)
        {
            if (fromUtc.HasValue)
                query = query.Where(ExpressionGreaterThanOrEqual(selector, fromUtc.Value));

            if (toUtc.HasValue)
                query = query.Where(ExpressionLessThan(selector, toUtc.Value));

            return query;
        }

        private static System.Linq.Expressions.Expression<Func<T, bool>> ExpressionGreaterThanOrEqual<T>(
            System.Linq.Expressions.Expression<Func<T, DateTime>> selector,
            DateTime value)
        {
            var body = System.Linq.Expressions.Expression.GreaterThanOrEqual(
                selector.Body,
                System.Linq.Expressions.Expression.Constant(value));
            return System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(body, selector.Parameters);
        }

        private static System.Linq.Expressions.Expression<Func<T, bool>> ExpressionLessThan<T>(
            System.Linq.Expressions.Expression<Func<T, DateTime>> selector,
            DateTime value)
        {
            var body = System.Linq.Expressions.Expression.LessThan(
                selector.Body,
                System.Linq.Expressions.Expression.Constant(value));
            return System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(body, selector.Parameters);
        }

        private static IReadOnlyList<T> ApplyKeyword<T>(
            IEnumerable<T> rows,
            string? keyword,
            Func<T, string> haystackFactory)
        {
            var tokens = Tokenize(keyword);
            if (tokens.Length == 0)
                return rows.ToArray();

            return rows
                .Where(row =>
                {
                    var haystack = InventoryQuestionParser.Normalize(haystackFactory(row));
                    return tokens.All(haystack.Contains);
                })
                .ToArray();
        }

        private static IReadOnlyList<InventoryChatProductStockDto> ApplyKeyword(
            IEnumerable<InventoryChatProductStockDto> rows,
            string? keyword)
            => ApplyKeyword(rows, keyword, x => $"{x.Code} {x.Name} {x.CategoryName}");

        private static IReadOnlyList<TReceipt> ApplyReceiptKeyword<TReceipt>(
            IEnumerable<TReceipt> receipts,
            string? keyword)
            where TReceipt : BaseEntity
        {
            var tokens = Tokenize(keyword);
            if (tokens.Length == 0)
                return receipts.ToArray();

            return receipts
                .Where(receipt =>
                {
                    var details = receipt switch
                    {
                        ImportReceipt importReceipt => string.Join(" ", importReceipt.Details.Select(x => $"{x.ProductCode} {x.ProductName}")),
                        ExportReceipt exportReceipt => string.Join(" ", exportReceipt.Details.Select(x => $"{x.ProductCode} {x.ProductName}")),
                        _ => string.Empty
                    };

                    var partner = receipt switch
                    {
                        ImportReceipt importReceipt => importReceipt.SupplierName,
                        ExportReceipt exportReceipt => exportReceipt.CustomerName,
                        _ => null
                    };

                    var haystack = InventoryQuestionParser.Normalize($"{receipt.Code} {partner} {details}");
                    return tokens.All(haystack.Contains);
                })
                .ToArray();
        }

        private static string[] Tokenize(string? keyword)
            => InventoryQuestionParser.Normalize(keyword ?? string.Empty)
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(x => x.Length >= 2)
                .Distinct()
                .ToArray();

        private static string BuildProductsSummary(IEnumerable<ImportReceiptDetail> details)
            => BuildProductsSummary(details
                .Where(x => !x.IsDeleted)
                .Select(x => (x.ProductCode, x.ProductName, x.Quantity)));

        private static string BuildProductsSummary(IEnumerable<ExportReceiptDetail> details)
            => BuildProductsSummary(details
                .Where(x => !x.IsDeleted)
                .Select(x => (x.ProductCode, x.ProductName, x.Quantity)));

        private static string BuildProductsSummary(IEnumerable<(string? Code, string Name, int Quantity)> rows)
        {
            var values = rows
                .Take(4)
                .Select(x => $"{x.Code ?? "#"} {x.Name} x{x.Quantity:N0}")
                .ToList();

            return values.Count == 0 ? "-" : string.Join("; ", values);
        }

        private static string MapInventoryTransactionType(InventoryTransactionType type)
            => type switch
            {
                InventoryTransactionType.Import => "Serial nhập kho",
                InventoryTransactionType.Export => "Serial xuất kho",
                InventoryTransactionType.Return => "Serial trả kho",
                InventoryTransactionType.Adjust => "Điều chỉnh serial",
                InventoryTransactionType.Reserve => "Giữ serial",
                InventoryTransactionType.Release => "Nhả giữ serial",
                _ => "Giao dịch serial"
            };

        private static string MapHoldingTransactionType(HoldingTransactionType type)
            => type switch
            {
                HoldingTransactionType.Borrow => "Kỹ thuật mượn",
                HoldingTransactionType.Return => "Kỹ thuật trả",
                HoldingTransactionType.ConvertToSale => "Kỹ thuật đã dùng/bán",
                HoldingTransactionType.ManualAdjustment => "Điều chỉnh kỹ thuật",
                _ => "Giao dịch kỹ thuật"
            };
    }
}
