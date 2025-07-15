namespace BizMate.Domain.Entities
{
    public abstract class BaseCoreEntity
    {
        public Guid Id { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; } = DateTime.UtcNow;
        public Guid? UpdatedBy { get; set; }
        public uint RowVersion { get; set; } = 1;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
    }

    public abstract class BaseEntity : BaseCoreEntity
    {
        public string Code { get; set; } = default!;
        public Guid StoreId { get; set; }
        public Store Store { get; set; } = default!;
        public bool IsActive { get; set; } = true;
    }
}
