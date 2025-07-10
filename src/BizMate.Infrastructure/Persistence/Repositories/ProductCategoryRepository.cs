using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SqlKata;

namespace BizMate.Infrastructure.Persistence.Repositories
{
    public class ProductCategoryRepository : IProductCategoryRepository
    {
        private readonly AppDbContext _context;

        public ProductCategoryRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(ProductCategory producCategory, CancellationToken cancellationToken = default)
        {
            await _context.ProductCategories.AddAsync(producCategory, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var product = await _context.ProductCategories.FindAsync(id);
            if (product is not null && !product.IsDeleted)
            {
                product.IsDeleted = true;
                product.DeletedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<(List<ProductCategory>, int TotalCount)> GetAllAsync(Guid storeId, CancellationToken cancellationToken = default)
        {
            var result = await _context.ProductCategories
                .Where(c => !c.IsDeleted && c.StoreId == storeId)
                .ToListAsync(cancellationToken);

            var totalCount = result.Count();

            return (result, totalCount);
        }

        public async Task<ProductCategory> GetByIdAsync(Guid storeId, Guid id, CancellationToken cancellationToken)
        {
            return await _context.ProductCategories
            .Where(p => !p.IsDeleted && p.Id == id && p.StoreId == storeId)
            .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ProductCategory> GetByNameAsync(string name, CancellationToken cancellationToken)
        {
            return await _context.ProductCategories
                .Where(p => !p.IsDeleted && p.Name == name)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task UpdateAsync(ProductCategory producCategory, CancellationToken cancellationToken = default)
        {
            var entry = _context.Entry(producCategory);
            entry.Property(nameof(BaseEntity.RowVersion)).OriginalValue = producCategory.RowVersion;
            producCategory.RowVersion++;

            try
            {
                _context.ProductCategories.Update(producCategory);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException("Dữ liệu đã bị thay đổi bởi người khác. Vui lòng tải lại.");
            }
        }
    }
}
