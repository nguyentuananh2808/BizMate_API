using BizMate.Application.Common.Security;

namespace BizMate.Public.Auth
{
    public class UserSession : IUserSession
    {
        public Guid? UserId { get; set; }
        public string UserName { get; set; } = default!;
        public string Role { get; set; } = default!;
        public Guid StoreId { get; set; }
        public string AccessToken { get; set; }

        public bool IsValid() =>
            UserId.HasValue && !string.IsNullOrWhiteSpace(Role);
    }

}
