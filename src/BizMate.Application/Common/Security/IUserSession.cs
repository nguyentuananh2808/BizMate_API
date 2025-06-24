namespace BizMate.Application.Common.Security
{
    public interface IUserSession
    {
        Guid UserId { get; }
        string? UserName { get; }
        string? Role { get; }
        Guid StoreId { get; }
        string? AccessToken { get; }
        bool IsAuthenticated { get; }
    }
}
