using AutoMapper;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using BizMate.Domain.Constants;
using BizMate.Public.Dto.UserAggregate;
using BizMate.Public.Message;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using _User = BizMate.Domain.Entities.User;

namespace BizMate.Application.UserCases.User.Queries.UserLogin
{
    public sealed class UserLoginHandler : IRequestHandler<UserLoginRequest, UserLoginResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtFactory _jwtFactory;
        private readonly ITokenFactory _tokenFactory;
        private readonly IUserSession _userSession;
        private readonly IMapper _mapper;
        private readonly ILogger<UserLoginHandler> _logger;
        private readonly IStringLocalizer<UserLoginHandler> _localizer;


        public UserLoginHandler(IJwtFactory jwtFactory, ITokenFactory tokenFactory, IUserSession userSession,
            IMapper mapper, ILogger<UserLoginHandler> logger, IDistributedCache cache, IStringLocalizer<UserLoginHandler> localizer, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _jwtFactory = jwtFactory;
            _tokenFactory = tokenFactory;
            _userSession = userSession;
            _mapper = mapper;
            _logger = logger;
            _localizer = localizer;
        }

        public async Task<UserLoginResponse> Handle(UserLoginRequest request, CancellationToken cancellationToken)
        {
            var response = await UserLogin(request);
            return response;
        }

        public async Task<UserLoginResponse> UserLogin(UserLoginRequest request)
        {
            var email = request.Email;
            var password = request.Password;

            #region check email exist
            var emailDb = await _userRepository.GetByEmailAsync(email, default);
            if (emailDb == null)
            {
                var message = CommonAppMessageUtils.NotExist<_User>(request.Email, _localizer);
                _logger.LogWarning(message);
                return new UserLoginResponse(success: false, message: message);
            }
            #endregion

            #region Check password
            var isValidPassword = PasswordHasher.Verify(password, emailDb.PasswordHash, emailDb.PasswordSalt);
            if (!isValidPassword)
            {
                var message = CommonAppMessageUtils.Invalid<_User>(request.Password, _localizer);
                _logger.LogWarning(message);
                return new UserLoginResponse(success: false, message: message);
            }
            #endregion

            #region Generate token
            var generateToken = _tokenFactory.GenerateToken();
            var accessToken = await _jwtFactory.GenerateEncodedToken(emailDb);
            #endregion
            var userDto = _mapper.Map<UserCoreDto>(emailDb);
            return new UserLoginResponse(accessToken, userDto);
        }
    }
}
