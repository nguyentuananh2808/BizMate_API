using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Infrastructure.Migrations;
using Microsoft.EntityFrameworkCore;

namespace BizMate.Infrastructure.Persistence.Repositories
{
    public sealed class InventoryOverviewRepository : IInventoryOverviewRepository
    {
        private readonly AppDbContext _context;

        public InventoryOverviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CategoryGroupExistsAsync(
            Guid storeId,
            Guid categoryGroupId,
            bool includeInactive,
            CancellationToken cancellationToken = default)
        {
            var query = _context.CategoryGroups
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.StoreId == storeId && x.Id == categoryGroupId);

            if (!includeInactive)
            {
                query = query.Where(x => !x.IsActive);
            }

            return await query.AnyAsync(cancellationToken);
        }

        public async Task<List<InventoryOverviewRowDto>> GetOverviewRowsAsync(
            Guid storeId,
            Guid? categoryGroupId,
            string? keyword,
            bool includeInactive,
            CancellationToken cancellationToken = default)
        {
            var products = _context.Products
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.StoreId == storeId);

            if (!includeInactive)
            {
                products = products.Where(x => !x.IsActive);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var normalizedKeyword = keyword.Trim().ToLower();
                products = products.Where(x =>
                    x.Code.ToLower().Contains(normalizedKeyword) ||
                    x.Name.ToLower().Contains(normalizedKeyword));
            }

            var categories = _context.ProductCategories
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.StoreId == storeId);

            if (!includeInactive)
            {
                categories = categories.Where(x => !x.IsActive);
            }

            var categoryGroups = _context.CategoryGroups
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.StoreId == storeId);

            if (!includeInactive)
            {
                categoryGroups = categoryGroups.Where(x => !x.IsActive);
            }

            var stocks = _context.Stocks
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.StoreId == storeId);

            var query =
                from product in products
                join category in categories
                    on product.ProductCategoryId equals category.Id into productCategoryJoin
                from category in productCategoryJoin.DefaultIfEmpty()
                join groupItem in categoryGroups
                    on category.CategoryGroupId equals groupItem.Id into categoryGroupJoin
                from groupItem in categoryGroupJoin.DefaultIfEmpty()
                join stock in stocks
                    on product.Id equals stock.ProductId into stockJoin
                from stock in stockJoin.DefaultIfEmpty()
                where !categoryGroupId.HasValue || category.CategoryGroupId == categoryGroupId.Value
                select new InventoryOverviewRowDto
                {
                    ProductId = product.Id,
                    ProductCode = product.Code,
                    ProductName = product.Name,
                    Unit = product.Unit,
                    IsSerialTracked = product.IsSerialTracked,
                    ProductCategoryId = category == null ? null : category.Id,
                    ProductCategoryName = category == null ? null : category.Name,
                    ProductCategoryCreatedDate = category == null ? null : category.CreatedDate,
                    CategoryGroupId = groupItem == null ? null : groupItem.Id,
                    CategoryGroupName = groupItem == null ? null : groupItem.Name,
                    CategoryGroupCreatedDate = groupItem == null ? null : groupItem.CreatedDate,
                    StockQuantity = stock == null ? 0 : stock.Quantity,
                    ReservedQuantity = stock == null ? 0 : stock.Reserved,
                    AvailableQuantity = stock == null ? 0 : stock.Quantity - stock.Reserved,
                    UpdatedDate = product.UpdatedDate
                };

            return await query.ToListAsync(cancellationToken);
        }
    }
}
