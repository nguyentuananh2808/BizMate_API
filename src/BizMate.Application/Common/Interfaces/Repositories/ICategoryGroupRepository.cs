using BizMate.Domain.Entities;

namespace BizMate.Application.Common.Interfaces.Repositories
{
    public interface ICategoryGroupRepository
    {
        Task<(List<CategoryGroup> CategoryGroups, int TotalCount)> SearchAsync(
            Guid storeId,
            string? keyword,
            int pageIndex,
            int pageSize,
            bool? isActive,
            CancellationToken cancellationToken);

        Task<List<CategoryGroup>> GetAllAsync(Guid storeId, CancellationToken cancellationToken);
        Task<CategoryGroup?> GetByIdAsync(Guid storeId, Guid id, CancellationToken cancellationToken);
        Task<CategoryGroup?> GetByNameAsync(Guid storeId, string name, Guid? excludeId, CancellationToken cancellationToken);
        Task AddAsync(CategoryGroup categoryGroup, CancellationToken cancellationToken);
        Task UpdateAsync(CategoryGroup categoryGroup, CancellationToken cancellationToken);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> HasProductCategoriesAsync(Guid storeId, Guid id, CancellationToken cancellationToken);
        Task AssignProductCategoriesAsync(Guid storeId, Guid categoryGroupId, IReadOnlyCollection<Guid> productCategoryIds, CancellationToken cancellationToken);
    }
}
