namespace BizMate.Domain.Entities
{
    public class UserPermission : BaseCoreEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = default!;

        public Guid StoreId { get; set; }
        public Store Store { get; set; } = default!;

        public Guid PermissionId { get; set; }
        public Permission Permission { get; set; } = default!;
    }
}
