using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BizMate.Infrastructure.Persistence.Repositories
{
    public class OtpVerificationRepository : IOtpVerificationRepository
    {
        private readonly AppDbContext _context;

        public OtpVerificationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddOtpAsync(string email, string otpCode, DateTime expiredAt, CancellationToken cancellationToken)
        {
            var otp = new OtpVerification
            {
                Id = Guid.NewGuid(),
                Email = email,
                OtpCode = otpCode,
                ExpiredAt = expiredAt,
                IsUsed = false,
                IsDeleted = false,
                RowVersion = Guid.NewGuid().ToByteArray()
            };

            await _context.OtpVerifications.AddAsync(otp, cancellationToken);
            await _context.SaveChangesAsync();
        }

        public async Task<OtpVerification?> GetValidOtpAsync(string email, string otpCode)
        {
            return await _context.OtpVerifications
                .Where(x => x.Email == email
                            && x.OtpCode == otpCode
                            && !x.IsUsed
                            && x.ExpiredAt > DateTime.UtcNow
                            && !x.IsDeleted)
                .OrderByDescending(x => x.ExpiredAt)
                .FirstOrDefaultAsync();
        }

        public async Task MarkOtpAsUsedAsync(Guid otpId)
        {
            var otp = await _context.OtpVerifications.FindAsync(otpId);
            if (otp != null && !otp.IsDeleted)
            {
                otp.IsUsed = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task SoftDeleteOtpAsync(Guid otpId)
        {
            var otp = await _context.OtpVerifications.FindAsync(otpId);
            if (otp != null && !otp.IsDeleted)
            {
                otp.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<OtpVerification>> GetOtpsByEmailAsync(string email)
        {
            return await _context.OtpVerifications
                .Where(x => x.Email == email && !x.IsDeleted)
                .OrderByDescending(x => x.ExpiredAt)
                .ToListAsync();
        }
    }
}
