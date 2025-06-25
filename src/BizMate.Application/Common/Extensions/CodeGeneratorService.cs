using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Interfaces.Repositories;

namespace BizMate.Application.Common.Extensions
{
    public class CodeGeneratorService : ICodeGeneratorService
    {
        private readonly ICodeGeneratorRepository _repository;

        public CodeGeneratorService(ICodeGeneratorRepository repository)
        {
            _repository = repository;
        }

        public async Task<string> GenerateCodeAsync(string prefix, int numberLength = 5)
        {
            var number = await _repository.GetAndIncreaseLastNumberAsync(prefix);
            return $"{prefix}{number.ToString().PadLeft(numberLength, '0')}";
        }
    }
}
