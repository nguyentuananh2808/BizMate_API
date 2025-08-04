using System.ComponentModel.DataAnnotations;

namespace BizMate.Domain.Entities
{
    public abstract class BaseCoreEntity
    {
        public Guid Id { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
        public Guid? UpdatedBy { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
    }

    public abstract class Base : BaseCoreEntity
    {
        public Guid StoreId { get; set; }
        public Store Store { get; set; } = default!;
    }

    public abstract class BaseEntity : Base
    {
        public bool IsActive { get; set; } = false;
        public string Code { get; set; } = default!;
    }
}
