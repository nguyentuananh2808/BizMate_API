namespace BizMate.Application.Common.Interfaces.Repositories
{
    public interface IInventoryReceiptRepository
    {
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        Task AddAsync(InventoryReceipt receipt);
        Task<InventoryReceipt?> GetByIdAsync(Guid id);
    }
}
