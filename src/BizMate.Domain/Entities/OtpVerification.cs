namespace BizMate.Domain.Entities
{
    public class OtpVerification : BaseCoreEntity
    {
        public string Email { get; set; } = default!;
        public string OtpCode { get; set; } = default!;
        public DateTime ExpiredAt { get; set; }
        public bool IsUsed { get; set; }
    }

}
