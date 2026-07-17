using BizMate.Domain.Entities;

namespace BizMate.Application.Common.Interfaces.Repositories
{
    public interface IInventoryTransactionRepository
    {
        Task AddAsync(InventoryTransaction transaction, CancellationToken ct = default);
        Task AddRangeAsync(IEnumerable<InventoryTransaction> transactions, CancellationToken ct = default);

        /// <summary>
        /// Gets inventory movement history for a serial-tracked item.
        /// </summary>
        Task<List<InventoryTransaction>> GetByProductItemIdAsync(
            Guid productItemId, CancellationToken ct = default);
    }
}
