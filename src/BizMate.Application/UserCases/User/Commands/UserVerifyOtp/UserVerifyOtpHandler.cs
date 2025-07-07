using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using BizMate.Domain.Entities;
using BizMate.Public.Dto.UserAggregate;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using AutoMapper;
using _User = BizMate.Domain.Entities.User;
using BizMate.Application.Resources;

namespace BizMate.Application.UserCases.User.Commands.UserVerifyOtp
{
    public class UserVerifyOtpHandler : IRequestHandler<UserVerifyOtpRequest, UserVerifyOtpResponse>
    {
        private readonly IAppMessageService _messageService;
        private readonly IUserRepository _userRepository;
        private readonly IOtpStore _otpStore;
        private readonly ILogger<UserVerifyOtpHandler> _logger;
        private readonly IStringLocalizer<MessageUtils> _localizer;
        private readonly IMapper _mapper;

        #region constructor
        public UserVerifyOtpHandler(
            IAppMessageService messageService,
            IUserRepository userRepository,
            IOtpStore otpStore,
            ILogger<UserVerifyOtpHandler> logger,
            IStringLocalizer<MessageUtils> localizer,
            IMapper mapper)
        {
            _messageService = messageService;
            _userRepository = userRepository;
            _otpStore = otpStore;
            _logger = logger;
            _localizer = localizer;
            _mapper = mapper;
        }
        #endregion

        public async Task<UserVerifyOtpResponse> Handle(UserVerifyOtpRequest request, CancellationToken cancellationToken)
        {
            return await VerifyOtpAsync(request, cancellationToken);
        }

        private async Task<UserVerifyOtpResponse> VerifyOtpAsync(UserVerifyOtpRequest request, CancellationToken cancellationToken)
        {
            var email = request.Email;
            var inputOtp = request.Otp;

            #region get data OTP for Redis
            var otpData = await _otpStore.GetOtpAsync(email);
            if (otpData == null)
            {
                var msg = _localizer["Mã OTP đã hết hạn hoặc không tồn tại"];
                _logger.LogWarning(msg);
                return new UserVerifyOtpResponse(false, msg);
            }

            if (otpData.Otp != inputOtp)
            {
                var msg = _localizer["Mã OTP không đúng"];
                _logger.LogWarning(msg);
                return new UserVerifyOtpResponse(false, msg);
            }
            #endregion

            #region check email exists
            var existingUser = await _userRepository.GetByEmailAsync(email, cancellationToken);
            if (existingUser != null)
            {
                var msg = _messageService.AlreadyExist(existingUser, _localizer);
                _logger.LogWarning(msg);
                return new UserVerifyOtpResponse(false, msg);
            }
            #endregion

            #region Hash password
            var (hashedPassword, salt) = PasswordHasher.HashWithSalt(request.Password);
            #endregion

            #region create store and user
            var store = new Store
            {
                Id = Guid.NewGuid(),
                Name = otpData.StoreName
            };

            var user = new _User
            {
                Id = Guid.NewGuid(),
                Email = otpData.Email,
                FullName = otpData.FullName,
                Role = "Owner",
                PasswordHash = hashedPassword,
                PasswordSalt = salt,
                Store = store,
                StoreId = store.Id,
                RowVersion = 1
            };
            await _userRepository.AddAsync(user, cancellationToken);
            #endregion

            await _otpStore.RemoveOtpAsync(email);

            var userDto = _mapper.Map<UserCoreDto>(user);
            return new UserVerifyOtpResponse(userDto, true, _localizer["Xác thực OTP thành công"]);
        }
    }
}
