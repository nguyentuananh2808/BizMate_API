namespace BizMate.Application.Common.Security
{
    public interface ITokenFactory
    {
        string GenerateToken(int size = 32);
    }
}
