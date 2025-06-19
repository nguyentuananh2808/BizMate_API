namespace BizMate.Public.Dto.UserAggregate
{
    public class UserCoreDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Role { get; set; }
        public string StoreName { get; set; } = default!;
    }
}
