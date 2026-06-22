using AutoMapper;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using BizMate.Application.UserCases.User.Commands.UserRegister;
using BizMate.Domain.Entities;
using BizMate.Public.Dto.UserAggregate;
using BizMate.Public.Message;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BizMate.Application.UserCases.User.Commands.UserVerifyOtp
{
    public sealed class UserVerifyOtpHandler(
        ICodeGeneratorService codeGeneratorService,
        IAppMessageService messageService,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        IOtpVerificationRepository otpVerificationRepository,
        IOtpStore otpStore,
        IUnitOfWork unitOfWork,
        ILogger<UserVerifyOtpHandler> logger,
        IMapper mapper)
        : IRequestHandler<UserVerifyOtpRequest, UserVerifyOtpResponse>
    {
        public async Task<UserVerifyOtpResponse> Handle(
            UserVerifyOtpRequest request,
            CancellationToken cancellationToken)
        {
            var email = request.Email.Trim().ToLowerInvariant();
            var inputOtp = request.Otp.Trim();
            var otpData = await otpStore.GetOtpAsync(email);

            if (otpData is null ||
                !string.Equals(otpData.Purpose, OtpPurpose.Registration, StringComparison.Ordinal))
            {
                var message = messageService.OtpNotExist();
                logger.LogWarning("OTP đăng ký không tồn tại cho {Email}.", email);
                return new UserVerifyOtpResponse(false, message);
            }

            if (!string.Equals(otpData.Otp, inputOtp, StringComparison.Ordinal))
            {
                var message = messageService.OtpInvalid();
                logger.LogWarning("OTP đăng ký không hợp lệ cho {Email}.", email);
                return new UserVerifyOtpResponse(false, message);
            }

            var persistedOtp = await otpVerificationRepository.GetValidOtpAsync(email, inputOtp);
            if (persistedOtp is null)
                return new UserVerifyOtpResponse(false, messageService.OtpInvalid());

            var existingUser = await userRepository.GetByEmailAsync(email, cancellationToken);
            if (existingUser is not null)
                return new UserVerifyOtpResponse(false, ValidationMessage.LocalizedStrings.AlreadyExist);

            var ownerRole = await roleRepository.GetByNameAsync("Owner", cancellationToken);
            if (ownerRole is null)
            {
                logger.LogError("Không tìm thấy role hệ thống Owner khi đăng ký {Email}.", email);
                return new UserVerifyOtpResponse(
                    false,
                    "Hệ thống chưa được khởi tạo vai trò chủ công ty.");
            }

            var (hashedPassword, salt) = PasswordHasher.HashWithSalt(otpData.Password);
            var store = new Store
            {
                Id = Guid.NewGuid(),
                Name = otpData.StoreName.Trim(),
                CreatedDate = DateTime.UtcNow
            };
            var userId = Guid.NewGuid();
            var user = new BizMate.Domain.Entities.User
            {
                Id = userId,
                CreatedBy = userId,
                Code = await codeGeneratorService.GenerateCodeAsync("#U", 5),
                Email = email,
                FullName = otpData.FullName.Trim(),
                Role = ownerRole.Name,
                PasswordHash = hashedPassword,
                PasswordSalt = salt,
                Store = store,
                StoreId = store.Id,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            await unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                await userRepository.AddAsync(user, cancellationToken);
                await userRoleRepository.AddAsync(new UserRole
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    RoleId = ownerRole.Id,
                    StoreId = store.Id,
                    CreatedBy = user.Id,
                    CreatedDate = DateTime.UtcNow
                }, cancellationToken);

                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                logger.LogError(ex, "Không thể tạo công ty và tài khoản chủ {Email}.", email);
                return new UserVerifyOtpResponse(
                    false,
                    "Đã xảy ra lỗi khi tạo công ty. Vui lòng thử lại.");
            }

            await otpVerificationRepository.MarkOtpAsUsedAsync(persistedOtp.Id);
            await otpVerificationRepository.SoftDeleteOtpAsync(persistedOtp.Id);
            await otpStore.RemoveOtpAsync(email);

            var userDto = mapper.Map<UserCoreDto>(user);
            return new UserVerifyOtpResponse(
                userDto,
                true,
                "Xác thực OTP và tạo công ty thành công.");
        }
    }
}
