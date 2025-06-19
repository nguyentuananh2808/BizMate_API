namespace BizMate.Domain.Constants
{
    public class AuthConstants
    {
        public const string Bearer = "Bearer";
        public static class JwtClaimIdentifiers
        {
            public const string IdentityId = "iid";
            public const string UserId = "uid";
            public const string UserRole = "role";
            public const string UserNumber = "number";
            public const string Name = "name";
        }
    }
}
