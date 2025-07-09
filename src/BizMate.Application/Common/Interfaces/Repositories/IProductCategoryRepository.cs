using BizMate.Domain.Entities;

namespace BizMate.Application.Common.Interfaces.Repositories
{
    public interface IProductCategoryRepository
    {
        Task<(List<ProductCategory>, int TotalCount)> GetAllAsync(Guid storeId, CancellationToken cancellationToken);
        Task<ProductCategory> GetByIdAsync(Guid storeId, Guid id, CancellationToken cancellationToken);
        Task<ProductCategory> GetByNameAsync(string name, CancellationToken cancellationToken);
        Task AddAsync(ProductCategory producCategory, CancellationToken cancellationToken);
        Task UpdateAsync(ProductCategory producCategory, CancellationToken cancellationToken);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
    }
}
