using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Domain.Entities;
using BizMate.Infrastructure.Migrations;
using Microsoft.EntityFrameworkCore;

namespace BizMate.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var normalizedEmail = email.Trim().ToLower();
            return await _context.Users
                .Include(x => x.Store) 
                .FirstOrDefaultAsync(
                    u => u.Email.ToLower() == normalizedEmail && !u.IsDeleted,
                    cancellationToken);
        }

        public async Task<User?> GetByIdInStoreAsync(
            Guid userId,
            Guid storeId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Include(x => x.Store)
                .Include(x => x.UserRoles)
                    .ThenInclude(x => x.Role)
                .Include(x => x.UserPermissions)
                .FirstOrDefaultAsync(
                    u => u.Id == userId && u.StoreId == storeId && !u.IsDeleted,
                    cancellationToken);
        }

        public async Task<bool> ExistsInStoreAsync(
            Guid userId,
            Guid storeId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Users.AnyAsync(
                u => u.Id == userId && u.StoreId == storeId && !u.IsDeleted,
                cancellationToken);
        }

        public async Task<(List<User> Users, int TotalCount)> SearchUsersWithPagingAsync(
            Guid storeId,
            string? keyword,
            int pageIndex,
            int pageSize,
            bool? isActive,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Users
                .Include(u => u.Store)
                .Include(u => u.UserRoles)
                    .ThenInclude(u => u.Role)
                .Include(u => u.UserPermissions)
                .Where(u => u.StoreId == storeId && !u.IsDeleted);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var key = keyword.Trim().ToLower();
                query = query.Where(u =>
                    u.FullName.ToLower().Contains(key)
                    || u.Email.ToLower().Contains(key)
                    || u.Code.ToLower().Contains(key));
            }

            if (isActive.HasValue)
                query = query.Where(u => u.IsActive == isActive.Value);

            var totalCount = await query.CountAsync(cancellationToken);
            var users = await query
                .OrderByDescending(u => u.CreatedDate)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (users, totalCount);
        }

        public async Task<bool> ExistsByEmailAsync(
            string email,
            Guid? excludeUserId = null,
            CancellationToken cancellationToken = default)
        {
            var normalizedEmail = email.Trim().ToLower();
            return await _context.Users.AnyAsync(
                u => u.Email.ToLower() == normalizedEmail
                   && !u.IsDeleted
                   && (!excludeUserId.HasValue || u.Id != excludeUserId.Value),
                cancellationToken);
        }

        public async Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            if (user.Store != null)
            {
                _context.Stores.Add(user.Store); 
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken); 
        }

        public Task UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            _context.Users.Update(user);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(User user, CancellationToken cancellationToken = default)
        {
            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;
            user.UpdatedDate = DateTime.UtcNow;
            _context.Users.Update(user);
            return Task.CompletedTask;
        }
    }
}
