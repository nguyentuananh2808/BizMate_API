using BizMate.Application.Common.Security;
using BizMate.Domain.Entities;
using BizMate.Public.Dto.Identity;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BizMate.Infrastructure.Security
{
    public class JwtFactory : IJwtFactory
    {
        private readonly JwtSecurityTokenHandler _jwtTokenHandler;
        private readonly JwtOptions _jwtOptions;

        public JwtFactory(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
            _jwtTokenHandler = new JwtSecurityTokenHandler();
        }

        public async Task<AccessToken> GenerateEncodedToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),

                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("store", user.Store?.Name ?? string.Empty)
            };

            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: null,
                expires: _jwtOptions.Expiration,
                signingCredentials: _jwtOptions.SigningCredentials
            );

            var encodedJwt = _jwtTokenHandler.WriteToken(jwt);
            return new AccessToken(encodedJwt, _jwtOptions.ExpirySeconds);
        }

        private static long ToUnixEpochDate(DateTime dateTime)
        {
            return (long)Math.Round((dateTime.ToUniversalTime() - DateTime.UnixEpoch).TotalSeconds);
        }
    }
}
