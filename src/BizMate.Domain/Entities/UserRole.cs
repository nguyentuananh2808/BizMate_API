namespace BizMate.Domain.Entities
{
    /// <summary>
    /// Gán Role cho User trong phạm vi một Store cụ thể.
    /// Một User có thể có Role khác nhau ở các Store khác nhau.
    /// </summary>
    public class UserRole : BaseCoreEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = default!;

        public Guid RoleId { get; set; }
        public Role Role { get; set; } = default!;

        public Guid StoreId { get; set; }
        public Store Store { get; set; } = default!;
    }
}