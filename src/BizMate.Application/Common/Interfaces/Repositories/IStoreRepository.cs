using BizMate.Domain.Entities;

namespace BizMate.Application.Common.Interfaces.Repositories
{
    public interface IStoreRepository
    {
        Task<Store?> GetByNameAsync(string storeName, CancellationToken cancellationToken = default);
        Task AddAsync(Store store, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(string storeName, CancellationToken cancellationToken = default);
    }
}
