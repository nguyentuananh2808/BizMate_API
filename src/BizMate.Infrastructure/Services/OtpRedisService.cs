using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.User.Commands.UserRegister;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace BizMate.Infrastructure.Services
{
    public class OtpRedisService : IOtpStore
    {
        private readonly IDistributedCache _cache;

        public OtpRedisService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task SaveOtpAsync(string email, TempOtpUserData data, TimeSpan expiresIn, CancellationToken cancellationToken)
        {
            var json = JsonConvert.SerializeObject(data);
            await _cache.SetStringAsync($"otp:{email}", json, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiresIn
            }, cancellationToken);
        }

        public async Task<TempOtpUserData?> GetOtpAsync(string email)
        {
            var json = await _cache.GetStringAsync($"otp:{email}");
            return json == null ? null : JsonConvert.DeserializeObject<TempOtpUserData>(json);
        }

        public async Task RemoveOtpAsync(string email)
        {
            await _cache.RemoveAsync($"otp:{email}");
        }
    }

}
