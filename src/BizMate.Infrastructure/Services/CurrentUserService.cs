using BizMate.Application.Common.Security;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BizMate.Infrastructure.Services
{
    public class CurrentUserService : IUserSession
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string UserId => _httpContextAccessor.HttpContext?.User?.FindFirst("user_id")?.Value
         ?? throw new UnauthorizedAccessException("UserId not found");

        public string? UserName =>
            _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

        public string? Email =>
            _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;

        public string? Role =>
            _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;

        public Guid StoreId =>
            Guid.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst("store_id")?.Value, out var storeId)
                ? storeId
                : throw new UnauthorizedAccessException("StoreId not found");

        public string? AccessToken =>
            _httpContextAccessor.HttpContext?.Request?.Headers["Authorization"].ToString()?.Replace("Bearer ", "");

        public bool IsAuthenticated =>
            _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    }
}
