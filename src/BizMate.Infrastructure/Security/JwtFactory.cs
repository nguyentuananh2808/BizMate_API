using BizMate.Application.Common.Security;
using BizMate.Domain.Entities;
using BizMate.Infrastructure.Persistence;
using BizMate.Public.Dto.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BizMate.Infrastructure.Security
{
    public class JwtFactory : IJwtFactory
    {
        private readonly JwtSecurityTokenHandler _jwtTokenHandler;
        private readonly JwtOptions _jwtOptions;
        private readonly AppDbContext _context;

        public JwtFactory(IOptions<JwtOptions> jwtOptions, AppDbContext context)
        {
            _jwtOptions       = jwtOptions.Value;
            _jwtTokenHandler  = new JwtSecurityTokenHandler();
            _context          = context;
        }

        public async Task<AccessToken> GenerateEncodedToken(User user)
        {
            // 1. Query tất cả permission của user trong store này
            //    UserRole → Role → RolePermission → Permission.Name
            var rolePermissions = _context.UserRoles
                .Where(ur => ur.UserId == user.Id
                          && ur.StoreId == user.StoreId
                          && !ur.IsDeleted
                          && !ur.Role.IsDeleted)
                .SelectMany(ur => ur.Role.RolePermissions
                    .Where(rp => !rp.IsDeleted && !rp.Permission.IsDeleted))
                .Select(rp => rp.Permission.Name);

            var directPermissions = _context.UserPermissions
                .Where(up => up.UserId == user.Id
                          && up.StoreId == user.StoreId
                          && !up.IsDeleted
                          && !up.Permission.IsDeleted)
                .Select(up => up.Permission.Name);

            var permissions = await rolePermissions
                .Concat(directPermissions)
                .Distinct()
                .ToListAsync();

            // 2. Base claims (giữ nguyên các claim cũ để không breaking change)
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),

                new Claim("user_id",    user.Id.ToString()),
                new Claim("name",       user.FullName),
                new Claim("email",      user.Email),
                new Claim("role",       user.Role),         // giữ lại claim cũ
                new Claim("store_name", user.Store.Name),
                new Claim("store_id",   user.StoreId.ToString()),
            };

            // 3. Nhúng từng permission vào claims
            //    FE/middleware check: claims.Where(c => c.Type == "permission")
            foreach (var perm in permissions)
                claims.Add(new Claim("permission", perm));

            // 4. Ký token
            var jwt = new JwtSecurityToken(
                issuer:             _jwtOptions.Issuer,
                audience:           _jwtOptions.Audience,
                claims:             claims,
                notBefore:          DateTime.UtcNow,
                expires:            _jwtOptions.Expiration,
                signingCredentials: _jwtOptions.SigningCredentials
            );

            var encodedJwt = _jwtTokenHandler.WriteToken(jwt);
            return new AccessToken(encodedJwt, _jwtOptions.ExpirySeconds);
        }

        private static long ToUnixEpochDate(DateTime dateTime)
            => (long)Math.Round((dateTime.ToUniversalTime() - DateTime.UnixEpoch).TotalSeconds);
    }
}
