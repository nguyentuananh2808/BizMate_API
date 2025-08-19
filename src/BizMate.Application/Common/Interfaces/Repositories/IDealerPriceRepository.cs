using BizMate.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BizMate.Application.Common.Interfaces.Repositories
{
    public interface IDealerPriceRepository
    {
        Task AddAsync(DealerPrice dealerPrice, CancellationToken cancellationToken = default);
        Task UpdateAsync(DealerPrice dealerPrice, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<DealerPrice?> CheckDealerPriceExist(Guid storeId, Guid productId, Guid dealerLevelId);
        Task<DealerPrice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    }
}
