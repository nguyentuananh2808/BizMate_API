namespace BizMate.Domain.Entities
{
    public class Role : BaseCoreEntity
    {
        public string Name { get; set; } = default!;
        public string DisplayName { get; set; } = default!;

        /// <summary>
        /// Marks protected roles that cannot be renamed or deleted from the UI.
        /// </summary>
        public bool IsSystem { get; set; } = false;

        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
