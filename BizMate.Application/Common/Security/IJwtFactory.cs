using BizMate.Domain.Entities;
using BizMate.Public.Dto.Identity;

namespace BizMate.Application.Common.Security
{
    public interface IJwtFactory
    {
        Task<AccessToken> GenerateEncodedToken(User user);
    }
}
