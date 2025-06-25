using BizMate.Application.Common.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BizMate.Infrastructure.Persistence.Repositories
{
    public class InventoryReceiptRepository : IInventoryReceiptRepository
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction _currentTransaction;
        public async Task BeginTransactionAsync()
        {
            _currentTransaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _currentTransaction.CommitAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            await _currentTransaction.RollbackAsync();
        }
        public InventoryReceiptRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(InventoryReceipt receipt)
        {
            await _context.InventoryReceipts.AddAsync(receipt);
            await _context.SaveChangesAsync();
        }

        public async Task<InventoryReceipt?> GetByIdAsync(Guid id)
        {
            return await _context.InventoryReceipts
                .Include(r => r.Details)
                .FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}
