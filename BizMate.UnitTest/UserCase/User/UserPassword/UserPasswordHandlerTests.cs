using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using BizMate.Application.UserCases.User.Commands.UserPassword;
using BizMate.Application.UserCases.User.Commands.UserRegister;
using BizMate.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace BizMate.UnitTest.UserCase.User.UserPassword
{
    public class UserPasswordHandlerTests
    {
        [Fact]
        public async Task ForgotPassword_ShouldSendPasswordResetOtp_WhenUserExists()
        {
            var userRepository = new Mock<IUserRepository>();
            var otpRepository = new Mock<IOtpVerificationRepository>();
            var otpStore = new Mock<IOtpStore>();
            var emailService = new Mock<IEmailService>();
            var logger = new Mock<ILogger<ForgotPasswordHandler>>();
            userRepository
                .Setup(repository => repository.GetByEmailAsync(
                    "owner@bizmate.vn",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new BizMate.Domain.Entities.User
                {
                    Id = Guid.NewGuid(),
                    Email = "owner@bizmate.vn",
                    FullName = "Owner"
                });
            otpStore
                .Setup(store => store.GetOtpAsync("owner@bizmate.vn"))
                .ReturnsAsync((TempOtpUserData?)null);

            var handler = new ForgotPasswordHandler(
                userRepository.Object,
                otpRepository.Object,
                otpStore.Object,
                emailService.Object,
                logger.Object);

            var response = await handler.Handle(
                new ForgotPasswordRequest { Email = "OWNER@BIZMATE.VN" },
                CancellationToken.None);

            response.Success.Should().BeTrue();
            otpStore.Verify(
                store => store.SaveOtpAsync(
                    "owner@bizmate.vn",
                    It.Is<TempOtpUserData>(data =>
                        data.Purpose == OtpPurpose.PasswordReset &&
                        data.Email == "owner@bizmate.vn"),
                    It.IsAny<TimeSpan>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            emailService.Verify(
                service => service.SendOtpEmailAsync(
                    "owner@bizmate.vn",
                    It.IsAny<string>(),
                    It.IsAny<DateTime>()),
                Times.Once);
        }

        [Fact]
        public async Task ResetPassword_ShouldUpdateHashAndInvalidateOtp_WhenOtpIsValid()
        {
            const string email = "owner@bizmate.vn";
            const string otp = "123456";
            const string newPassword = "NewPassword@123";
            var user = new BizMate.Domain.Entities.User
            {
                Id = Guid.NewGuid(),
                Email = email,
                FullName = "Owner",
                PasswordHash = "old",
                PasswordSalt = "old"
            };
            var persistedOtp = new OtpVerification
            {
                Id = Guid.NewGuid(),
                Email = email,
                OtpCode = otp,
                ExpiredAt = DateTime.UtcNow.AddMinutes(5)
            };

            var userRepository = new Mock<IUserRepository>();
            var otpRepository = new Mock<IOtpVerificationRepository>();
            var otpStore = new Mock<IOtpStore>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var logger = new Mock<ILogger<ResetPasswordHandler>>();
            otpStore
                .Setup(store => store.GetOtpAsync(email))
                .ReturnsAsync(new TempOtpUserData
                {
                    Email = email,
                    Otp = otp,
                    Purpose = OtpPurpose.PasswordReset,
                    CreatedAtUtc = DateTime.UtcNow
                });
            otpRepository
                .Setup(repository => repository.GetValidOtpAsync(email, otp))
                .ReturnsAsync(persistedOtp);
            userRepository
                .Setup(repository => repository.GetByEmailAsync(
                    email,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var handler = new ResetPasswordHandler(
                userRepository.Object,
                otpRepository.Object,
                otpStore.Object,
                unitOfWork.Object,
                logger.Object);

            var response = await handler.Handle(
                new ResetPasswordRequest
                {
                    Email = email,
                    Otp = otp,
                    NewPassword = newPassword
                },
                CancellationToken.None);

            response.Success.Should().BeTrue();
            PasswordHasher.Verify(
                newPassword,
                user.PasswordHash,
                user.PasswordSalt).Should().BeTrue();
            otpRepository.Verify(
                repository => repository.MarkOtpAsUsedAsync(persistedOtp.Id),
                Times.Once);
            otpRepository.Verify(
                repository => repository.SoftDeleteOtpAsync(persistedOtp.Id),
                Times.Once);
            otpStore.Verify(store => store.RemoveOtpAsync(email), Times.Once);
        }
    }
}
