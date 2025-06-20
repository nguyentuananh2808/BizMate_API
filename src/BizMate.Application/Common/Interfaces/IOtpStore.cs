using BizMate.Application.UserCases.User.Commands.UserRegister;

namespace BizMate.Application.Common.Interfaces
{
    public interface IOtpStore
    {
        Task SaveOtpAsync(string email, TempOtpUserData data, TimeSpan expiresIn);
        Task<TempOtpUserData?> GetOtpAsync(string email);
        Task RemoveOtpAsync(string email);
    }

}
