
namespace BizMate.Domain.Entities
{
    public class OtpVerification 
    {
        public string Email { get; set; } = default!;
        public string OtpCode { get; set; } = default!;
        public DateTime ExpiredAt { get; set; }
        public bool IsUsed { get; set; }
        public Guid Id { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; } = DateTime.UtcNow;
        public Guid? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
    }

}
