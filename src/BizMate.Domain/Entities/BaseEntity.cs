namespace BizMate.Domain.Entities
{
    public class BaseEntity
    {
        public Guid Id { get; set; }
        public uint RowVersion { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
    }
}
