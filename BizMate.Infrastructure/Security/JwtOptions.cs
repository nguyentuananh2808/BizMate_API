using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BizMate.Infrastructure.Security
{
    public class JwtOptions
    {
        public string Issuer { get; set; } = default!;
        public string Audience { get; set; } = default!;
        public string SecretKey { get; set; } = default!;
        public int ExpirySeconds { get; set; }
        public DateTime IssuedAt => DateTime.UtcNow;

        public DateTime Expiration => DateTime.UtcNow.AddSeconds(ExpirySeconds);

        public Func<Task<string>> JtiGenerator => () => Task.FromResult(Guid.NewGuid().ToString());

        public SigningCredentials SigningCredentials =>
            new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey)),
                SecurityAlgorithms.HmacSha256);
    }
}
