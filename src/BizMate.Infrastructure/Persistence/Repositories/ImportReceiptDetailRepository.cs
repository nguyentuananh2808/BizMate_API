using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BizMate.Infrastructure.Persistence.Repositories
{
    public class ImportReceiptDetailRepository : IImportReceiptDetailRepository
    {
        private readonly AppDbContext _context;

        public ImportReceiptDetailRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ImportReceiptDetail>> GetByReceiptIdAsync(Guid receiptId, CancellationToken cancellationToken = default)
        {
            return await _context.ImportReceiptDetails
                .Where(d => d.ImportReceiptId == receiptId)
                .ToListAsync(cancellationToken);
        }

        public async Task<ImportReceiptDetail?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.ImportReceiptDetails
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        }

        public async Task AddAsync(ImportReceiptDetail detail, CancellationToken cancellationToken = default)
        {
            await _context.ImportReceiptDetails.AddAsync(detail, cancellationToken);
        }

        public async Task AddRangeAsync(IEnumerable<ImportReceiptDetail> details, CancellationToken cancellationToken = default)
        {
            await _context.ImportReceiptDetails.AddRangeAsync(details, cancellationToken);
        }

        public Task UpdateAsync(ImportReceiptDetail detail, CancellationToken cancellationToken = default)
        {
            _context.ImportReceiptDetails.Update(detail);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var detail = await _context.ImportReceiptDetails.FindAsync(new object[] { id }, cancellationToken);
            if (detail != null)
            {
                _context.ImportReceiptDetails.Remove(detail);
            }
        }

        public async Task DeleteRangeAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
        {
            var details = await _context.ImportReceiptDetails
                .Where(d => ids.Contains(d.Id))
                .ToListAsync(cancellationToken);

            if (details.Any())
            {
                _context.ImportReceiptDetails.RemoveRange(details);
            }
        }
    }
}