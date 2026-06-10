namespace BizMate.Domain.Entities
{
    public class Permission : BaseCoreEntity
    {
        public string Name { get; set; } = default!;        // "order.create"
        public string DisplayName { get; set; } = default!; // "Tạo đơn hàng"
        public string Group { get; set; } = default!;       // "order", "product"...

        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
        public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
    }
}
