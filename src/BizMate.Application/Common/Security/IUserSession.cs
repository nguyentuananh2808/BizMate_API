namespace BizMate.Application.Common.Security
{
    public interface IUserSession
    {
        string? AccessToken { get; set; }
    }
}
