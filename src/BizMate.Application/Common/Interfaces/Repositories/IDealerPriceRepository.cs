using BizMate.Domain.Entities;

namespace BizMate.Application.Common.Interfaces.Repositories
{
    public interface IDealerPriceRepository
    {
        Task AddRangeAsync(IEnumerable<DealerPrice> dealerPrices, CancellationToken cancellationToken = default);
        Task UpdateAsync(DealerPrice dealerPrice, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> CheckDealerPricesExistAsync(Guid storeId, IEnumerable<Guid> productIds, Guid dealerLevelId);
        Task<DealerPrice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    }
}
