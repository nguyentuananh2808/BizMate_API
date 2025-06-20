using BizMate.Domain.Entities;

namespace BizMate.Application.Common.Interfaces.Repositories
{
    public interface IOtpVerificationRepository
    {
        Task AddOtpAsync(string email, string otpCode, DateTime expiredAt, CancellationToken cancellationToken);
        Task<OtpVerification?> GetValidOtpAsync(string email, string otpCode);
        Task MarkOtpAsUsedAsync(Guid otpId);
    }

}
