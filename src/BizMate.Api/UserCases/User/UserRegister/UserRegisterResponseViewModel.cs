namespace BizMate.Api.UserCases.User.UserRegister
{
    public class UserRegisterResponseViewModel
    {
        public string FullName { get; set; }
        public string NameStore { get; set; }
        public string Email { get; set; }
        public DateTime OtpExpiredAt { get; set; }

        public UserRegisterResponseViewModel(string fullName, string nameStore, string email, DateTime otpExpiredAt)
        {
            FullName = fullName;
            NameStore = nameStore;
            Email = email;
            OtpExpiredAt = otpExpiredAt;
        }
    }
}
