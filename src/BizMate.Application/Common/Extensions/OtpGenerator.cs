namespace BizMate.Application.Common.Extensions
{
    public static class OtpGenerator
    {
        public static string Generate(int length = 6)
        {
            var random = new Random();
            return string.Concat(Enumerable.Range(0, length).Select(_ => random.Next(0, 10)));
        }
    }

}
