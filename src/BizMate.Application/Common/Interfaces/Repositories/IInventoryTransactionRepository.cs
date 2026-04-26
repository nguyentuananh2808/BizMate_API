// FILE: src/BizMate.Application/Common/Interfaces/Repositories/IInventoryTransactionRepository.cs
using BizMate.Domain.Entities;

namespace BizMate.Application.Common.Interfaces.Repositories
{
    public interface IInventoryTransactionRepository
    {
        Task AddAsync(InventoryTransaction transaction, CancellationToken ct = default);
        Task AddRangeAsync(IEnumerable<InventoryTransaction> transactions, CancellationToken ct = default);

        /// <summary>Lịch sử giao dịch của 1 ProductItem.</summary>
        Task<List<InventoryTransaction>> GetByProductItemIdAsync(
            Guid productItemId, CancellationToken ct = default);
    }
}
