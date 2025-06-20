namespace BizMate.Domain.Entities
{
    public class OtpVerification
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string OtpCode { get; set; }
        public DateTime ExpiredAt { get; set; }
        public bool IsUsed { get; set; }
    }

}
