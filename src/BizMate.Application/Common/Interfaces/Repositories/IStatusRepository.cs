namespace BizMate.Application.Common.Interfaces.Repositories
{
    public interface IStatusRepository
    {
        Task<Guid> GetIdByGroupAndCodeAsync(string code,string group);
    }
}
