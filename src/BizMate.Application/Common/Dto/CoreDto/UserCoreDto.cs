using BizMate.Domain.Entities;

namespace BizMate.Public.Dto.UserAggregate
{
    public class UserCoreDto : BaseEntity
    {
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Role { get; set; } = default!;
        public string StoreName { get; set; } = default!;
    }
}
