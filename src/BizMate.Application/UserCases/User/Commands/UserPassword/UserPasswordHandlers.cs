using BizMate.Application.Common.Extensions;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using BizMate.Application.UserCases.User.Commands.UserRegister;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BizMate.Application.UserCases.User.Commands.UserPassword
{
    public sealed class ForgotPasswordHandler(
        IUserRepository userRepository,
        IOtpVerificationRepository otpVerificationRepository,
        IOtpStore otpStore,
        IEmailService emailService,
        ILogger<ForgotPasswordHandler> logger)
        : IRequestHandler<ForgotPasswordRequest, ForgotPasswordResponse>
    {
        public async Task<ForgotPasswordResponse> Handle(
            ForgotPasswordRequest request,
            CancellationToken cancellationToken)
        {
            var email = request.Email.Trim().ToLowerInvariant();
            var user = await userRepository.GetByEmailAsync(email, cancellationToken);

            if (user is null)
                return new ForgotPasswordResponse(false, "Email không tồn tại trong hệ thống.");

            var otpCode = OtpGenerator.Generate(6);
            var expiredAt = DateTime.UtcNow.AddMinutes(5);
            var otpData = new TempOtpUserData
            {
                Email = email,
                FullName = user.FullName,
                Otp = otpCode,
                Purpose = OtpPurpose.PasswordReset
            };

            await otpVerificationRepository.AddOtpAsync(
                email,
                otpCode,
                expiredAt,
                cancellationToken);

            await otpStore.SaveOtpAsync(
                email,
                otpData,
                TimeSpan.FromMinutes(5),
                cancellationToken);

            try
            {
                await emailService.SendOtpEmailAsync(email, otpCode, expiredAt);
            }
            catch (Exception ex)
            {
                await otpStore.RemoveOtpAsync(email);
                logger.LogError(ex, "Không thể gửi OTP đặt lại mật khẩu cho {Email}.", email);
                return new ForgotPasswordResponse(
                    false,
                    "Không thể gửi OTP. Vui lòng kiểm tra cấu hình email và thử lại.");
            }

            return new ForgotPasswordResponse(email, expiredAt);
        }
    }

    public sealed class ResetPasswordHandler(
        IUserRepository userRepository,
        IOtpVerificationRepository otpVerificationRepository,
        IOtpStore otpStore,
        IUnitOfWork unitOfWork,
        ILogger<ResetPasswordHandler> logger)
        : IRequestHandler<ResetPasswordRequest, ResetPasswordResponse>
    {
        public async Task<ResetPasswordResponse> Handle(
            ResetPasswordRequest request,
            CancellationToken cancellationToken)
        {
            var email = request.Email.Trim().ToLowerInvariant();
            var inputOtp = request.Otp.Trim();
            var otpData = await otpStore.GetOtpAsync(email);

            if (otpData is null ||
                !string.Equals(otpData.Purpose, OtpPurpose.PasswordReset, StringComparison.Ordinal) ||
                !string.Equals(otpData.Otp, inputOtp, StringComparison.Ordinal))
            {
                return new ResetPasswordResponse(
                    false,
                    "OTP không hợp lệ hoặc đã hết hạn.");
            }

            var persistedOtp = await otpVerificationRepository.GetValidOtpAsync(email, inputOtp);
            if (persistedOtp is null)
                return new ResetPasswordResponse(false, "OTP không hợp lệ hoặc đã được sử dụng.");

            var user = await userRepository.GetByEmailAsync(email, cancellationToken);
            if (user is null)
                return new ResetPasswordResponse(false, "Không tìm thấy tài khoản cần đặt lại mật khẩu.");

            try
            {
                var (passwordHash, passwordSalt) = PasswordHasher.HashWithSalt(request.NewPassword);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                user.UpdatedDate = DateTime.UtcNow;

                await userRepository.UpdateAsync(user, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await otpVerificationRepository.MarkOtpAsUsedAsync(persistedOtp.Id);
                await otpVerificationRepository.SoftDeleteOtpAsync(persistedOtp.Id);
                await otpStore.RemoveOtpAsync(email);

                return new ResetPasswordResponse(
                    true,
                    "Đặt lại mật khẩu thành công. Bạn có thể đăng nhập bằng mật khẩu mới.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Đặt lại mật khẩu thất bại cho {Email}.", email);
                return new ResetPasswordResponse(
                    false,
                    "Không thể đặt lại mật khẩu. Vui lòng thử lại.");
            }
        }
    }
}
