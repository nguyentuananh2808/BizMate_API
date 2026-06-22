namespace BizMate.Application.UserCases.User.Commands.UserRegister
{
    public static class OtpPurpose
    {
        public const string Registration = "Registration";
        public const string PasswordReset = "PasswordReset";
    }

    public class TempOtpUserData
    {
        public string Email { get; set; } = default!;
        public string FullName { get; set; } = string.Empty;
        public string StoreName { get; set; } = string.Empty;
        public string Otp { get; set; } = default!;
        public string Password { get; set; } = string.Empty;
        public string Purpose { get; set; } = OtpPurpose.Registration;
    }
}
