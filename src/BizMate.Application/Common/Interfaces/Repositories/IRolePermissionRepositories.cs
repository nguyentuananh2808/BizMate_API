using BizMate.Domain.Entities;

namespace BizMate.Application.Common.Interfaces.Repositories
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<Role?> GetByNameAsync(string name, CancellationToken ct = default);
        Task<Role?> GetByIdWithPermissionsAsync(Guid id, CancellationToken ct = default);
        Task<List<Role>> GetAllWithCountsAsync(CancellationToken ct = default);
        Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default);
        Task AddAsync(Role role, CancellationToken ct = default);
        Task UpdateAsync(Role role, CancellationToken ct = default);
        Task DeleteAsync(Role role, CancellationToken ct = default);
    }

    public interface IPermissionRepository
    {
        Task<List<Permission>> GetAllAsync(CancellationToken ct = default);
        Task<List<Permission>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
    }

    public interface IUserRoleRepository
    {
        Task<List<UserRole>> GetByUserIdAndStoreIdAsync(Guid userId, Guid storeId, CancellationToken ct = default);
        Task<List<UserRole>> GetByUserIdAndStoreIdWithPermissionsAsync(Guid userId, Guid storeId, CancellationToken ct = default);
        Task<List<UserRole>> GetByRoleIdAsync(Guid roleId, CancellationToken ct = default);
        Task<bool> ExistsAsync(Guid userId, Guid roleId, Guid storeId, CancellationToken ct = default);
        Task AddAsync(UserRole userRole, CancellationToken ct = default);
        Task DeleteAsync(UserRole userRole, CancellationToken ct = default);
        Task DeleteRangeAsync(IEnumerable<UserRole> userRoles, CancellationToken ct = default);
    }

    public interface IUserPermissionRepository
    {
        Task<List<UserPermission>> GetByUserIdAndStoreIdAsync(Guid userId, Guid storeId, CancellationToken ct = default);
        Task<bool> ExistsAsync(Guid userId, Guid storeId, Guid permissionId, CancellationToken ct = default);
        Task AddRangeAsync(IEnumerable<UserPermission> userPermissions, CancellationToken ct = default);
        Task DeleteAsync(UserPermission userPermission, CancellationToken ct = default);
        Task DeleteRangeAsync(IEnumerable<UserPermission> userPermissions, CancellationToken ct = default);
    }
}
