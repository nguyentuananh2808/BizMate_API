using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BizMate.Infrastructure.Persistence.Repositories
{
    public class StoreRepository : IStoreRepository
    {
        private readonly AppDbContext _context;

        public StoreRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Store?> GetByNameAsync(string storeName, CancellationToken cancellationToken = default)
        {
            return await _context.Stores
                .FirstOrDefaultAsync(s => s.Name.ToLower() == storeName.ToLower(), cancellationToken);
        }

        public async Task AddAsync(Store store, CancellationToken cancellationToken = default)
        {
            _context.Stores.Add(store);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> ExistsAsync(string storeName, CancellationToken cancellationToken = default)
        {
            return await _context.Stores
                .AnyAsync(s => s.Name.ToLower() == storeName.ToLower(), cancellationToken);
        }
    }
}
