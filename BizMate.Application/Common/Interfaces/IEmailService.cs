namespace BizMate.Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task SendOtpEmailAsync(string toEmail, string otpCode, DateTime expiredAt);
    }
}
