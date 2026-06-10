using BizMate.Domain.Entities;

namespace BizMate.Application.Common.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<User?> GetByIdInStoreAsync(Guid userId, Guid storeId, CancellationToken cancellationToken = default);
        Task<(List<User> Users, int TotalCount)> SearchUsersWithPagingAsync(
            Guid storeId,
            string? keyword,
            int pageIndex,
            int pageSize,
            bool? isActive,
            CancellationToken cancellationToken = default);
        Task<bool> ExistsByEmailAsync(
            string email,
            Guid? excludeUserId = null,
            CancellationToken cancellationToken = default);
        Task<bool> ExistsInStoreAsync(Guid userId, Guid storeId, CancellationToken cancellationToken = default);
        Task AddAsync(User user, CancellationToken cancellationToken = default);
        Task UpdateAsync(User user, CancellationToken cancellationToken = default);
        Task DeleteAsync(User user, CancellationToken cancellationToken = default);
    }
}
