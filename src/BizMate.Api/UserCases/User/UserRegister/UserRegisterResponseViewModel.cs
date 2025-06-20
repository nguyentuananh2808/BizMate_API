namespace BizMate.Api.UserCases.User.UserRegister
{
    public class UserRegisterResponseViewModel
    {
        public string Email { get; set; }
        public DateTime OtpExpiredAt { get; set; }

        public UserRegisterResponseViewModel(string email,DateTime otpExpiredAt)
        {
            Email = email;
            OtpExpiredAt = otpExpiredAt;
        }
    }
}
