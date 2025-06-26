namespace BizMate.Application.Common.Security
{
    public interface IUserSession
    {
        string UserId { get; }
        string? UserName { get; }
        string? Role { get; }
        Guid StoreId { get; }
        string? AccessToken { get; }
        bool IsAuthenticated { get; }
    }
}
