namespace BizMate.Domain.Entities
{
    public class Status
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Group { get; set; } = default!;
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; } = DateTime.UtcNow;
        public Guid? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public bool IsActive { get; set; } = false;
        public string Code { get; set; } = default!;
    }
}
