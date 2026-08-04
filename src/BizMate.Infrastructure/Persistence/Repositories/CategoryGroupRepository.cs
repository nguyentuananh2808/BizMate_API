using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Domain.Entities;
using BizMate.Infrastructure.Migrations;
using Microsoft.EntityFrameworkCore;

namespace BizMate.Infrastructure.Persistence.Repositories
{
    public class CategoryGroupRepository : ICategoryGroupRepository
    {
        private readonly AppDbContext _context;

        public CategoryGroupRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(List<CategoryGroup> CategoryGroups, int TotalCount)> SearchAsync(
            Guid storeId,
            string? keyword,
            int pageIndex,
            int pageSize,
            bool? isActive,
            CancellationToken cancellationToken)
        {
            var query = _context.CategoryGroups
                .AsNoTracking()
                .Include(x => x.ProductCategories.Where(c => !c.IsDeleted))
                .Where(x => !x.IsDeleted && x.StoreId == storeId);

            if (isActive.HasValue)
                query = query.Where(x => x.IsActive == isActive.Value);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var normalizedKeyword = keyword.Trim().ToLower();
                query = query.Where(x =>
                    x.Name.ToLower().Contains(normalizedKeyword) ||
                    x.Code.ToLower().Contains(normalizedKeyword));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var categoryGroups = await query
                .OrderByDescending(x => x.CreatedDate)
                .ThenByDescending(x => x.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (categoryGroups, totalCount);
        }

        public async Task<List<CategoryGroup>> GetAllAsync(Guid storeId, CancellationToken cancellationToken)
        {
            return await _context.CategoryGroups
                .AsNoTracking()
                .Include(x => x.ProductCategories.Where(c => !c.IsDeleted))
                .Where(x => !x.IsDeleted && x.StoreId == storeId)
                .OrderByDescending(x => x.CreatedDate)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<CategoryGroup?> GetByIdAsync(Guid storeId, Guid id, CancellationToken cancellationToken)
        {
            return await _context.CategoryGroups
                .Include(x => x.ProductCategories.Where(c => !c.IsDeleted))
                .Where(x => !x.IsDeleted && x.StoreId == storeId && x.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<CategoryGroup?> GetByNameAsync(Guid storeId, string name, Guid? excludeId, CancellationToken cancellationToken)
        {
            return await _context.CategoryGroups
                .Where(x =>
                    !x.IsDeleted &&
                    x.StoreId == storeId &&
                    x.Name == name &&
                    (!excludeId.HasValue || x.Id != excludeId.Value))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AddAsync(CategoryGroup categoryGroup, CancellationToken cancellationToken)
        {
            await _context.CategoryGroups.AddAsync(categoryGroup, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(CategoryGroup categoryGroup, CancellationToken cancellationToken)
        {
            _context.CategoryGroups.Update(categoryGroup);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var categoryGroup = await _context.CategoryGroups.FindAsync(new object?[] { id }, cancellationToken);
            if (categoryGroup is null || categoryGroup.IsDeleted)
                return;

            categoryGroup.IsDeleted = true;
            categoryGroup.DeletedAt = DateTime.UtcNow;
            categoryGroup.UpdatedDate = DateTime.UtcNow;
            categoryGroup.RowVersion = Guid.NewGuid();
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> HasProductCategoriesAsync(Guid storeId, Guid id, CancellationToken cancellationToken)
        {
            return await _context.ProductCategories
                .AnyAsync(x => !x.IsDeleted && x.StoreId == storeId && x.CategoryGroupId == id, cancellationToken);
        }

        public async Task AssignProductCategoriesAsync(
            Guid storeId,
            Guid categoryGroupId,
            IReadOnlyCollection<Guid> productCategoryIds,
            CancellationToken cancellationToken)
        {
            var categoryIds = productCategoryIds.Distinct().ToList();
            var currentCategories = await _context.ProductCategories
                .Where(x => !x.IsDeleted && x.StoreId == storeId && x.CategoryGroupId == categoryGroupId)
                .ToListAsync(cancellationToken);

            foreach (var category in currentCategories.Where(x => !categoryIds.Contains(x.Id)))
            {
                category.CategoryGroupId = null;
                category.UpdatedDate = DateTime.UtcNow;
                category.RowVersion = Guid.NewGuid();
            }

            if (categoryIds.Any())
            {
                var selectedCategories = await _context.ProductCategories
                    .Where(x => !x.IsDeleted && x.StoreId == storeId && categoryIds.Contains(x.Id))
                    .ToListAsync(cancellationToken);

                foreach (var category in selectedCategories)
                {
                    category.CategoryGroupId = categoryGroupId;
                    category.UpdatedDate = DateTime.UtcNow;
                    category.RowVersion = Guid.NewGuid();
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
