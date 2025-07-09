using BizMate.Application.Common.Extensions;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Resources;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace BizMate.Application.UserCases.User.Commands.UserRegister
{
    public sealed class UserRegisterHandler : IRequestHandler<UserRegisterRequest, UserRegisterResponse>
    {
        private readonly IAppMessageService _messageService;
        private readonly IUserRepository _userRepository;
        private readonly IOtpVerificationRepository _otpVerificationRepository;
        private readonly IOtpStore _otpStore;
        private readonly IEmailService _emailService;
        private readonly ILogger<UserRegisterHandler> _logger;
        private readonly IStringLocalizer<MessageUtils> _localizer;

        #region constructor
        public UserRegisterHandler(
            IAppMessageService messageService,
            IUserRepository userRepository,
            IOtpVerificationRepository otpVerificationRepository,
            IEmailService emailService,
            IOtpStore otpStore,
            ILogger<UserRegisterHandler> logger,
            IStringLocalizer<MessageUtils> localizer)
        {
            _messageService = messageService;
            _otpStore = otpStore;
            _userRepository = userRepository;
            _otpVerificationRepository = otpVerificationRepository;
            _emailService = emailService;
            _logger = logger;
            _localizer = localizer;
        }
        #endregion
        public async Task<UserRegisterResponse> Handle(UserRegisterRequest request, CancellationToken cancellationToken)
        {
            return await UserRegister(request, cancellationToken);
        }

        private async Task<UserRegisterResponse> UserRegister(UserRegisterRequest request, CancellationToken cancellationToken)
        {
            var email = request.Email;

            #region Check email exists
            var emailDb = await _userRepository.GetByEmailAsync(email, cancellationToken);
            if (emailDb != null)
            {
                var message = _messageService.AlreadyExist(email, _localizer);
                _logger.LogWarning(message);
                return new UserRegisterResponse(success: false, message: message);
            }
            #endregion

            #region Create & save OTP
            var otpCode = OtpGenerator.Generate(6);
            var expiredAt = DateTime.UtcNow.AddMinutes(5);

            await _otpVerificationRepository.AddOtpAsync(email, otpCode, expiredAt, cancellationToken);
            var tempData = new TempOtpUserData
            {
                Email = request.Email,
                FullName = request.FullName,
                StoreName = request.NameStore,
                Otp = otpCode
            };

            await _otpStore.SaveOtpAsync(request.Email, tempData, TimeSpan.FromMinutes(5), cancellationToken);

            try
            {
                await _emailService.SendOtpEmailAsync(email, otpCode, expiredAt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gửi OTP thất bại.");
                return new UserRegisterResponse(success: false, message: "Không thể gửi OTP. Vui lòng thử lại.");
            }
            #endregion

            return new UserRegisterResponse(tempData.FullName, tempData.StoreName, email, expiredAt);
        }
    }
}
