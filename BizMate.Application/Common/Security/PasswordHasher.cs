namespace BizMate.Application.Common.Security
{
    public static class PasswordHasher
    {
        public static bool Verify(string inputPassword, string storedHash, string storedSalt)
        {
            var saltBytes = Convert.FromBase64String(storedSalt);
            var hashedInput = Hash(inputPassword, saltBytes);
            return storedHash == hashedInput;
        }

        public static string Hash(string password, byte[] salt)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512(salt);
            var passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
            var computedHash = hmac.ComputeHash(passwordBytes);
            return Convert.ToBase64String(computedHash);
        }

        public static (string hash, string salt) HashWithSalt(string password)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512();
            var salt = hmac.Key;
            var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
        }
    }

}
