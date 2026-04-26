using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Domain.Entities;
using BizMate.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BizMate.Infrastructure.Persistence.Repositories
{
    // ── RoleRepository ────────────────────────────────────────────────────────
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _context;
        public RoleRepository(AppDbContext context) => _context = context;

        public async Task<Role?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _context.Roles.FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted, ct);

        public async Task<Role?> GetByIdWithPermissionsAsync(Guid id, CancellationToken ct = default)
            => await _context.Roles
                .Include(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
                .Include(r => r.UserRoles)
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted, ct);

        public async Task<List<Role>> GetAllWithCountsAsync(CancellationToken ct = default)
            => await _context.Roles
                .Where(r => !r.IsDeleted)
                .Include(r => r.RolePermissions)
                .Include(r => r.UserRoles)
                .OrderBy(r => r.Name)
                .ToListAsync(ct);

        public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default)
            => await _context.Roles.AnyAsync(r => r.Name == name && !r.IsDeleted, ct);

        public async Task AddAsync(Role role, CancellationToken ct = default)
            => await _context.Roles.AddAsync(role, ct);

        public Task UpdateAsync(Role role, CancellationToken ct = default)
        {
            _context.Roles.Update(role);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Role role, CancellationToken ct = default)
        {
            role.IsDeleted   = true;
            role.DeletedAt   = DateTime.UtcNow;
            role.UpdatedDate = DateTime.UtcNow;
            _context.Roles.Update(role);
            return Task.CompletedTask;
        }
    }

    // ── PermissionRepository ──────────────────────────────────────────────────
    public class PermissionRepository : IPermissionRepository
    {
        private readonly AppDbContext _context;
        public PermissionRepository(AppDbContext context) => _context = context;

        public async Task<List<Permission>> GetAllAsync(CancellationToken ct = default)
            => await _context.Permissions
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.Group).ThenBy(p => p.Name)
                .ToListAsync(ct);

        public async Task<List<Permission>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
            => await _context.Permissions
                .Where(p => ids.Contains(p.Id) && !p.IsDeleted)
                .ToListAsync(ct);
    }

    // ── UserRoleRepository ────────────────────────────────────────────────────
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly AppDbContext _context;
        public UserRoleRepository(AppDbContext context) => _context = context;

        public async Task<List<UserRole>> GetByUserIdAndStoreIdAsync(Guid userId, Guid storeId, CancellationToken ct = default)
            => await _context.UserRoles
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == userId && ur.StoreId == storeId && !ur.IsDeleted)
                .ToListAsync(ct);

        public async Task<List<UserRole>> GetByRoleIdAsync(Guid roleId, CancellationToken ct = default)
            => await _context.UserRoles
                .Where(ur => ur.RoleId == roleId && !ur.IsDeleted)
                .ToListAsync(ct);

        public async Task<bool> ExistsAsync(Guid userId, Guid roleId, Guid storeId, CancellationToken ct = default)
            => await _context.UserRoles.AnyAsync(
                ur => ur.UserId == userId && ur.RoleId == roleId && ur.StoreId == storeId && !ur.IsDeleted, ct);

        public async Task AddAsync(UserRole userRole, CancellationToken ct = default)
            => await _context.UserRoles.AddAsync(userRole, ct);

        public Task DeleteAsync(UserRole userRole, CancellationToken ct = default)
        {
            userRole.IsDeleted   = true;
            userRole.DeletedAt   = DateTime.UtcNow;
            userRole.UpdatedDate = DateTime.UtcNow;
            _context.UserRoles.Update(userRole);
            return Task.CompletedTask;
        }
    }
}
