// FILE: src/BizMate.Infrastructure/Persistence/Repositories/InventoryTransactionRepository.cs

using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Domain.Entities;
using BizMate.Infrastructure.Migrations;
using Microsoft.EntityFrameworkCore;

namespace BizMate.Infrastructure.Persistence.Repositories
{
    public class InventoryTransactionRepository : IInventoryTransactionRepository
    {
        private readonly AppDbContext _context;

        public InventoryTransactionRepository(AppDbContext context) => _context = context;

        public async Task AddAsync(InventoryTransaction transaction, CancellationToken ct = default)
            => await _context.InventoryTransactions.AddAsync(transaction, ct);

        public async Task AddRangeAsync(
            IEnumerable<InventoryTransaction> transactions, CancellationToken ct = default)
            => await _context.InventoryTransactions.AddRangeAsync(transactions, ct);

        public async Task<List<InventoryTransaction>> GetByProductItemIdAsync(
            Guid productItemId, CancellationToken ct = default)
            => await _context.InventoryTransactions
                .Where(t => t.ProductItemId == productItemId)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync(ct);
    }
}
