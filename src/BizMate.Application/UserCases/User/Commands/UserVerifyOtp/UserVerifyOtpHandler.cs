using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using BizMate.Domain.Entities;
using BizMate.Public.Dto.UserAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using AutoMapper;
using _User = BizMate.Domain.Entities.User;
using BizMate.Public.Message;

namespace BizMate.Application.UserCases.User.Commands.UserVerifyOtp
{
    public class UserVerifyOtpHandler : IRequestHandler<UserVerifyOtpRequest, UserVerifyOtpResponse>
    {
        private readonly IAppMessageService _messageService;
        private readonly IUserRepository _userRepository;
        private readonly IOtpStore _otpStore;
        private readonly ILogger<UserVerifyOtpHandler> _logger;
        private readonly ICodeGeneratorService _codeGeneratorService;
        private readonly IMapper _mapper;

        #region constructor
        public UserVerifyOtpHandler(
            ICodeGeneratorService codeGeneratorService,
            IAppMessageService messageService,
            IUserRepository userRepository,
            IOtpStore otpStore,
            ILogger<UserVerifyOtpHandler> logger,
            IMapper mapper)
        {
            _codeGeneratorService = codeGeneratorService;
            _messageService = messageService;
            _userRepository = userRepository;
            _otpStore = otpStore;
            _logger = logger;
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
                var msg = _messageService.OtpNotExist();
                _logger.LogWarning(msg);
                return new UserVerifyOtpResponse(false, msg);
            }

            if (otpData.Otp != inputOtp)
            {
                var msg = _messageService.OtpInvalid();
                _logger.LogWarning(msg);
                return new UserVerifyOtpResponse(false, msg);
            }
            #endregion

            #region check email exists
            var existingUser = await _userRepository.GetByEmailAsync(email, cancellationToken);
            if (existingUser != null)
            {
                var message = ValidationMessage.LocalizedStrings.AlreadyExist;
                _logger.LogWarning(message);
                return new UserVerifyOtpResponse(false, message);
            }
            #endregion

            #region Hash password
            var (hashedPassword, salt) = PasswordHasher.HashWithSalt(otpData.Password);
            #endregion

            #region create store and user
            string codeStore = await _codeGeneratorService.GenerateCodeAsync("#CH", 5);

            var store = new Store
            {
                Id = Guid.NewGuid(),
                Name = otpData.StoreName,
            };

            string code = await _codeGeneratorService.GenerateCodeAsync("#U", 5);

            var user = new _User
            {
                Id = Guid.NewGuid(),
                CreatedBy = Guid.NewGuid(),
                Code = code,
                Email = otpData.Email,
                FullName = otpData.FullName,
                Role = "Owner",
                PasswordHash = hashedPassword,
                PasswordSalt = salt,
                Store = store,
                StoreId = store.Id,
            };

            try
            {
                await _userRepository.AddAsync(user, cancellationToken);
            }
            catch (Exception ex)
            {
                var msg = "Đã xảy ra lỗi khi tạo người dùng. Vui lòng thử lại sau.";
                _logger.LogError(ex, "Lỗi không xác định khi tạo người dùng: {Email}", user.Email);
                return new UserVerifyOtpResponse(false, msg);
            }
            #endregion

            await _otpStore.RemoveOtpAsync(email);

            var userDto = _mapper.Map<UserCoreDto>(user);
            return new UserVerifyOtpResponse(userDto, true, "Xác thực OTP thành công");
        }

    }
}
