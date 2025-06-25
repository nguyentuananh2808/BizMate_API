using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BizMate.Infrastructure.Persistence.Repositories
{
    public class CodeGeneratorRepository : ICodeGeneratorRepository
    {
        private readonly AppDbContext _context;

        public CodeGeneratorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetAndIncreaseLastNumberAsync(string prefix)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            var sequence = await _context.CodeSequences
                .SingleOrDefaultAsync(x => x.Prefix == prefix);

            if (sequence == null)
            {
                sequence = new CodeSequence
                {
                    Prefix = prefix,
                    LastNumber = 1
                };
                _context.CodeSequences.Add(sequence);
            }
            else
            {
                sequence.LastNumber += 1;
                _context.CodeSequences.Update(sequence);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return sequence.LastNumber;
        }
    }
}
