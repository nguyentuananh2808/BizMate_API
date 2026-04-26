using BizMate.Domain.Entities;

namespace BizMate.Application.Common.Interfaces.Repositories
{
    public interface IProductItemRepository
    {
        Task<ProductItem?> GetBySerialNumberAsync(string serialNumber, CancellationToken ct = default);

        Task<List<ProductItem>> GetBySerialNumbersAsync(
            IEnumerable<string> serialNumbers,
            CancellationToken ct = default);

        Task<ProductItem?> GetByIdWithTransactionsAsync(Guid id, CancellationToken ct = default);

        Task<(List<ProductItem> Items, int TotalCount)> GetByProductAsync(
            Guid storeId,
            Guid productId,
            ProductItemStatus? status,
            int pageIndex,
            int pageSize,
            CancellationToken ct = default);

        Task<int> CountInStockAsync(Guid storeId, Guid productId, CancellationToken ct = default);

        Task<List<ProductItem>> GetInStockItemsAsync(
            Guid storeId,
            Guid productId,
            CancellationToken ct = default);

        Task<List<ProductItem>> GetByImportReceiptDetailIdsAsync(
            IEnumerable<Guid> importReceiptDetailIds,
            ProductItemStatus? status,
            CancellationToken ct = default);

        Task<List<ProductItem>> GetByOrderDetailIdsAsync(
            IEnumerable<Guid> orderDetailIds,
            ProductItemStatus? status,
            CancellationToken ct = default);

        Task<bool> ExistsBySerialNumberAsync(string serialNumber, CancellationToken ct = default);

        Task AddAsync(ProductItem item, CancellationToken ct = default);
        Task AddRangeAsync(IEnumerable<ProductItem> items, CancellationToken ct = default);
        Task UpdateAsync(ProductItem item, CancellationToken ct = default);
        Task UpdateRangeAsync(IEnumerable<ProductItem> items, CancellationToken ct = default);
    }
}
