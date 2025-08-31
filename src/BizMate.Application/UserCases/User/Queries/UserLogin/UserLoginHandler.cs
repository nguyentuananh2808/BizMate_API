using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using BizMate.Public.Message;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BizMate.Application.UserCases.User.Queries.UserLogin
{
    public sealed class UserLoginHandler : IRequestHandler<UserLoginRequest, UserLoginResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtFactory _jwtFactory;
        private readonly ITokenFactory _tokenFactory;
        private readonly ILogger<UserLoginHandler> _logger;

        #region constructor
        public UserLoginHandler(
            IJwtFactory jwtFactory, ITokenFactory tokenFactory, ILogger<UserLoginHandler> logger, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _jwtFactory = jwtFactory;
            _tokenFactory = tokenFactory;
            _logger = logger;
        }
        #endregion

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
                var message = ValidationMessage.LocalizedStrings.DataNotExist;
                _logger.LogWarning(message);
                return new UserLoginResponse(success: false, message);
            }
            #endregion

            #region Check password
            var isValidPassword = PasswordHasher.Verify(password, emailDb.PasswordHash, emailDb.PasswordSalt);
            if (!isValidPassword)
            {
                var message = ValidationMessage.LocalizedStrings.NotValidPassword;
                _logger.LogWarning(message);
                return new UserLoginResponse(success: false, message);
            }
            #endregion

            #region Generate token
            var generateToken = _tokenFactory.GenerateToken();
            var accessToken = await _jwtFactory.GenerateEncodedToken(emailDb);
            #endregion
            return new UserLoginResponse(accessToken);
        }
    }
}
