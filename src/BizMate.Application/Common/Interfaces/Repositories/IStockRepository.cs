using BizMate.Domain.Entities;

namespace BizMate.Application.Common.Interfaces.Repositories
{
    public interface IStockRepository
    {
        Task<List<Stock>> GetByStoreAndProductAsync(Guid storeId, List<Guid> productIds);
        Task AddAsync(Stock stock);
        Task UpdateAsync(IEnumerable<Stock> stocks, CancellationToken cancellationToken);
    }

}
