using BizMate.Domain.Entities;

namespace BizMate.Application.Common.Interfaces.Repositories
{
    public interface IStatusRepository
    {
        Task<Guid> GetIdByGroupAndCodeAsync(string code,string group);
        Task<Status?> GetIdById(Guid id,CancellationToken cancellation);
        Task<IEnumerable<Status>> GetStatusesOfGroup(string group);
    }
}
