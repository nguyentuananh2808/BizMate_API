namespace BizMate.Application.Common.Interfaces.Repositories
{
    public interface IStatusRepository
    {
        Task<Guid> GetStatusByCode(string code,string group);
    }
}
