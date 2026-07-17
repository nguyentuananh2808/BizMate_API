using BizMate.Application.Common.Security;
using BizMate.Domain.Entities;
using BizMate.Infrastructure.Migrations;
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
            var activeRoleName = await _context.UserRoles
                .Where(ur => ur.UserId == user.Id
                          && ur.StoreId == user.StoreId
                          && !ur.IsDeleted
                          && !ur.Role.IsDeleted)
                .OrderByDescending(ur => ur.CreatedDate)
                .Select(ur => ur.Role.Name)
                .FirstOrDefaultAsync();

            var roleName = string.IsNullOrWhiteSpace(activeRoleName)
                ? user.Role
                : activeRoleName;

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

            var legacyRolePermissions = _context.Roles
                .Where(role => role.Name == roleName && !role.IsDeleted)
                .SelectMany(role => role.RolePermissions
                    .Where(rolePermission =>
                        !rolePermission.IsDeleted &&
                        !rolePermission.Permission.IsDeleted))
                .Select(rolePermission => rolePermission.Permission.Name);

            var permissions = await rolePermissions
                .Concat(legacyRolePermissions)
                .Concat(directPermissions)
                .Distinct()
                .ToListAsync();

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),

                new Claim("user_id",    user.Id.ToString()),
                new Claim("name",       user.FullName),
                new Claim("email",      user.Email),
                new Claim("role",       roleName),
                new Claim("store_name", user.Store.Name),
                new Claim("store_id",   user.StoreId.ToString()),
            };

            foreach (var perm in permissions)
                claims.Add(new Claim("permission", perm));

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
