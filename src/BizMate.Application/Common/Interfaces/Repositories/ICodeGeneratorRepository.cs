namespace BizMate.Application.Common.Interfaces.Repositories
{
    public interface ICodeGeneratorRepository
    {
        Task<int> GetAndIncreaseLastNumberAsync(string prefix);
    }

}
