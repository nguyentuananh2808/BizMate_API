using BizMate.Application.UserCases.InventoryChat;

namespace BizMate.Application.Common.Interfaces.Repositories
{
    public interface IInventoryChatRepository
    {
        Task<IReadOnlyList<InventoryChatProductStockDto>> SearchProductsAsync(
            Guid storeId,
            string? keyword,
            int limit,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<InventoryChatProductStockDto>> GetLowStockProductsAsync(
            Guid storeId,
            int threshold,
            int limit,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<InventoryChatProductStockDto>> GetReservedStockProductsAsync(
            Guid storeId,
            string? keyword,
            int limit,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<InventoryChatProductStockDto>> GetSerialTrackedProductsAsync(
            Guid storeId,
            string? keyword,
            int limit,
            CancellationToken cancellationToken);

        Task<InventoryChatStockSummaryDto> GetStockSummaryAsync(
            Guid storeId,
            int lowStockThreshold,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<InventoryChatHoldingDto>> SearchTechnicianHoldingsAsync(
            Guid storeId,
            string? keyword,
            int limit,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<InventoryChatReceiptDto>> GetImportReceiptsAsync(
            Guid storeId,
            DateTime fromUtc,
            DateTime toUtc,
            string? keyword,
            int limit,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<InventoryChatReceiptDto>> GetExportReceiptsAsync(
            Guid storeId,
            DateTime fromUtc,
            DateTime toUtc,
            string? keyword,
            int limit,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<InventoryChatHistoryDto>> GetProductHistoryAsync(
            Guid storeId,
            string productKeyword,
            DateTime? fromUtc,
            DateTime? toUtc,
            int limit,
            CancellationToken cancellationToken);
    }
}
