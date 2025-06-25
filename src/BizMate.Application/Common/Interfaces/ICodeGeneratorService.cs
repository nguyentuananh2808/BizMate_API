namespace BizMate.Application.Common.Interfaces
{
    public interface ICodeGeneratorService
    {
        Task<string> GenerateCodeAsync(string prefix, int numberLength = 5);
    }

}
