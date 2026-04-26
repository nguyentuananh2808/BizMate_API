namespace BizMate.Domain.Entities
{
    public class Role : BaseCoreEntity
    {
        public string Name { get; set; } = default!;        // "Manager"
        public string DisplayName { get; set; } = default!; // "Quản lý"

        /// <summary>
        /// true = role hệ thống (Owner), không cho phép xóa hoặc sửa tên
        /// </summary>
        public bool IsSystem { get; set; } = false;

        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}