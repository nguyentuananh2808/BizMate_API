using BizMate.Domain.Constants;
using BizMate.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BizMate.Infrastructure.Migrations
{
    public static class AppDbContextSeed
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                await SeedStatusesAsync(context);
                await SeedPermissionsAsync(context);
                await context.SaveChangesAsync();
                await SeedRolesAsync(context);
                await SeedRolePermissionsAsync(context);

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // ─────────────────────────────────────────────────────────────
        // STATUS
        // ─────────────────────────────────────────────────────────────
        private static async Task SeedStatusesAsync(AppDbContext context)
        {
            var statuses = new List<Status>
            {
                new Status { Id = Guid.NewGuid(), Name = "Nháp", Code = "DRAFT", Group = "Order", IsActive = false },
                new Status { Id = Guid.NewGuid(), Name = "Tạo mới", Code = "NEW", Group = "Order", IsActive = false },
                new Status { Id = Guid.NewGuid(), Name = "Hoàn thành", Code = "COMPLETED", Group = "Order", IsActive = false },

                new Status { Id = Guid.NewGuid(), Name = "Tạo mới", Code = "NEW", Group = "ImportReceipt", IsActive = false },

                new Status { Id = Guid.NewGuid(), Name = "Còn bảo hành", Code = "AVAILABLE", Group = "Warranty", IsActive = false },
                new Status { Id = Guid.NewGuid(), Name = "Đã hết lượt bảo hành", Code = "USED_UP", Group = "Warranty", IsActive = false },
            };

            var existing = await context.Statuses
                .Select(s => new { s.Code, s.Group })
                .ToListAsync();

            var toAdd = statuses
                .Where(s => !existing.Any(e => e.Code == s.Code && e.Group == s.Group))
                .ToList();

            if (toAdd.Any())
                await context.Statuses.AddRangeAsync(toAdd);
        }

        // ─────────────────────────────────────────────────────────────
        // PERMISSION
        // ─────────────────────────────────────────────────────────────
        private static async Task SeedPermissionsAsync(AppDbContext context)
        {
            var existing = await context.Permissions
                .Select(p => p.Name)
                .ToListAsync();

            var toAdd = PermissionConstants.GetAll()
                .Where(p => !existing.Contains(p.Name))
                .Select(p => new Permission
                {
                    Id = Guid.NewGuid(), // có thể thay bằng deterministic nếu cần
                    Name = p.Name,
                    DisplayName = p.DisplayName,
                    Group = p.Group,
                    CreatedDate = DateTime.UtcNow,
                })
                .ToList();

            if (toAdd.Any())
                await context.Permissions.AddRangeAsync(toAdd);
        }

        // ─────────────────────────────────────────────────────────────
        // ROLE
        // ─────────────────────────────────────────────────────────────
        private static readonly Guid OwnerRoleId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        private static readonly Guid ManagerRoleId = Guid.Parse("00000000-0000-0000-0000-000000000002");
        private static readonly Guid StaffRoleId = Guid.Parse("00000000-0000-0000-0000-000000000003");
        private static readonly Guid WarehouseRoleId = Guid.Parse("00000000-0000-0000-0000-000000000004");
        private static readonly Guid TechnicianRoleId = Guid.Parse("00000000-0000-0000-0000-000000000005");

        private static async Task SeedRolesAsync(AppDbContext context)
        {
            var roles = new[]
            {
                new Role { Id = OwnerRoleId, Name = "Owner", DisplayName = "Chủ cửa hàng", IsSystem = true, CreatedDate = DateTime.UtcNow },
                new Role { Id = ManagerRoleId, Name = "Manager", DisplayName = "Quản lý", IsSystem = false, CreatedDate = DateTime.UtcNow },
                new Role { Id = StaffRoleId, Name = "Staff", DisplayName = "Nhân viên bán hàng", IsSystem = false, CreatedDate = DateTime.UtcNow },
                new Role { Id = WarehouseRoleId, Name = "Warehouse", DisplayName = "Thủ kho", IsSystem = false, CreatedDate = DateTime.UtcNow },
                new Role { Id = TechnicianRoleId, Name = "Technician", DisplayName = "Kỹ thuật viên", IsSystem = true, CreatedDate = DateTime.UtcNow },
            };

            var existingIds = await context.Roles.Select(r => r.Id).ToListAsync();

            var toAdd = roles
                .Where(r => !existingIds.Contains(r.Id))
                .ToList();

            if (toAdd.Any())
                await context.Roles.AddRangeAsync(toAdd);
        }

        // ─────────────────────────────────────────────────────────────
        // ROLE PERMISSION
        // ─────────────────────────────────────────────────────────────
        private static async Task SeedRolePermissionsAsync(AppDbContext context)
        {
            var allPerms = await context.Permissions.ToListAsync();

            Guid GetId(string name)
            {
                var perm = allPerms.FirstOrDefault(p => p.Name == name);
                if (perm == null)
                    throw new Exception($"Permission '{name}' chưa được seed");
                return perm.Id;
            }

            // Owner → tất cả
            await AddRolePerms(context, OwnerRoleId, allPerms.Select(p => p.Id).ToArray());

            // Manager → hạn chế
            await AddRolePerms(context, ManagerRoleId,
                allPerms.Where(p => p.Name is not (
                    PermissionConstants.User.Delete or
                    PermissionConstants.Role.Delete or
                    PermissionConstants.Role.Create))
                .Select(p => p.Id).ToArray());

            // Staff
            await AddRolePerms(context, StaffRoleId, new[]
            {
                GetId(PermissionConstants.Order.View),
                GetId(PermissionConstants.Order.Create),
                GetId(PermissionConstants.Order.Edit),
                GetId(PermissionConstants.Product.View),
                GetId(PermissionConstants.Stock.View),
                GetId(PermissionConstants.Customer.View),
                GetId(PermissionConstants.Customer.Create),
                GetId(PermissionConstants.Customer.Edit),
            });

            // Warehouse
            await AddRolePerms(context, WarehouseRoleId, new[]
            {
                GetId(PermissionConstants.Stock.View),
                GetId(PermissionConstants.Stock.Adjust),
                GetId(PermissionConstants.ImportReceipt.View),
                GetId(PermissionConstants.ImportReceipt.Create),
                GetId(PermissionConstants.ImportReceipt.Edit),
                GetId(PermissionConstants.ImportReceipt.Cancel),
                GetId(PermissionConstants.ExportReceipt.View),
                GetId(PermissionConstants.ExportReceipt.Create),
                GetId(PermissionConstants.ExportReceipt.Edit),
                GetId(PermissionConstants.ExportReceipt.Cancel),
                GetId(PermissionConstants.Product.View),
            });

            await AddRolePerms(context, TechnicianRoleId, new[]
            {
                GetId(PermissionConstants.Product.View),
                GetId(PermissionConstants.Stock.View),
            });
        }

        private static async Task AddRolePerms(AppDbContext context, Guid roleId, Guid[] permIds)
        {
            var existing = await context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => rp.PermissionId)
                .ToListAsync();

            var toAdd = permIds
                .Where(pid => !existing.Contains(pid))
                .Select(pid => new RolePermission
                {
                    Id = Guid.NewGuid(),
                    RoleId = roleId,
                    PermissionId = pid,
                    CreatedDate = DateTime.UtcNow,
                })
                .ToList();

            if (toAdd.Any())
                await context.RolePermissions.AddRangeAsync(toAdd);
        }
    }
}
