using BizMate.Domain.Entities;

namespace BizMate.Application.Common.Interfaces.Repositories
{
    public interface IStockRepository
    {
        Task<Stock> GetByStoreAndProductAsync(Guid storeId, Guid productId);
        Task AddAsync(Stock stock);
        Task UpdateAsync(Stock stock);
    }

}
