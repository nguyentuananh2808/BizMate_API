using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BizMate.Infrastructure.Persistence.Repositories
{
    public class DealerPriceRepository : IDealerPriceRepository
    {
        private readonly AppDbContext _context;

        public DealerPriceRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddRangeAsync(IEnumerable<DealerPrice> dealerPrices, CancellationToken cancellationToken = default)
        {
            await _context.AddRangeAsync(dealerPrices, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }


        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var DealerPrice = await _context.DealerPrices.FindAsync(id);
            if (DealerPrice is not null && !DealerPrice.IsDeleted)
            {
                DealerPrice.IsDeleted = true;
                DealerPrice.DeletedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellationToken);
            }
        }
        public async Task<DealerPrice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.DealerPrices
          .Where(p => !p.IsDeleted && p.Id == id)
          .FirstOrDefaultAsync(cancellationToken);
        }
        public async Task UpdateAsync(DealerPrice dealerPrice, CancellationToken cancellationToken = default)
        {
            _context.DealerPrices.Update(dealerPrice);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> CheckDealerPricesExistAsync(Guid storeId, IEnumerable<Guid> productIds, Guid dealerLevelId)
        {
            var exists = await _context.Set<DealerPrice>()
                 .AnyAsync(dp => dp.StoreId == storeId
                              && productIds.Contains(dp.ProductId)
                              && dp.DealerLevelId == dealerLevelId
                              && !dp.IsDeleted);

            // Nếu tìm thấy ít nhất 1 productId trùng => false
            return !exists;
        }

    }
}
