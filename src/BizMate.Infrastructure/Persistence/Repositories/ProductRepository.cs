using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SqlKata.Execution;

namespace BizMate.Infrastructure.Persistence.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Product> GetByIdAsync(Guid id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<List<Product>> SearchProducts(Guid storeId, Guid? supplierId, string? name,
            QueryFactory queryFactory)
        {

            var query = queryFactory.Query("Products as p");

            query.Where($"p.StoreId", storeId);

            if (supplierId.HasValue)
                query.Where($"p.SupplierId", supplierId.Value);

            if (!string.IsNullOrWhiteSpace(name))
                query.WhereRaw(@"LOWER(p.""Name"") LIKE ?", $"%{name.ToLower()}%");

            var result = await query.GetAsync<Product>();
            return result.ToList();
        }

        public async Task<(List<Product> Products, int TotalCount)> SearchProductsWithPaging(
            Guid storeId,
            string? keyword,
            int pageIndex,
            int pageSize,
            QueryFactory queryFactory)
        {
            var baseQuery = queryFactory.Query("Products as p")
                .Where("p.StoreId", storeId);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                baseQuery.WhereRaw(@"LOWER(p.""Name"") LIKE ?", $"%{keyword.ToLower()}%");
            }

            // Clone query to get total count
            var totalQuery = baseQuery.Clone();
            var totalCount = await totalQuery.CountAsync<int>();

            // Apply paging
            var results = await baseQuery
                .Offset((pageIndex - 1) * pageSize)
                .Limit(pageSize)
                .GetAsync<Product>();

            return (results.ToList(), totalCount);
        }


        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            var entry = _context.Entry(product);

            entry.Property(nameof(BaseEntity.RowVersion)).OriginalValue = product.RowVersion;

            product.RowVersion++;

            try
            {
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException("Dữ liệu đã bị thay đổi bởi người khác. Vui lòng tải lại.");
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product is not null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
    }
}
