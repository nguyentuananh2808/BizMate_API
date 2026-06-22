using BizMate.Domain.Entities;

namespace BizMate.Application.Common.Interfaces.Repositories
{
    public interface ITechnicianHoldingRepository
    {
        Task<Order?> GetOrderWithDetailsAsync(Guid orderId, Guid storeId, CancellationToken ct = default);
        Task<List<Technician>> SearchTechniciansAsync(Guid storeId, string? keyword, bool includeInactive = false, CancellationToken ct = default);
        Task<Technician?> GetTechnicianAsync(Guid technicianId, Guid storeId, CancellationToken ct = default);
        Task<List<Technician>> GetTechniciansByIdsAsync(Guid storeId, IEnumerable<Guid> technicianIds, CancellationToken ct = default);
        Task<bool> ExistsTechnicianPhoneAsync(Guid storeId, string phone, Guid? excludeId = null, CancellationToken ct = default);
        Task<List<Stock>> GetStocksAsync(Guid storeId, IEnumerable<Guid> productIds, CancellationToken ct = default);
        Task<TechnicianHolding?> GetHoldingAsync(Guid storeId, Guid technicianId, Guid productId, CancellationToken ct = default);
        Task<List<TechnicianHolding>> GetHoldingsByProductAsync(Guid storeId, Guid productId, CancellationToken ct = default);
        Task<List<TechnicianHolding>> GetHoldingsAsync(Guid storeId, Guid? technicianId = null, CancellationToken ct = default);
        Task<List<TechnicianHolding>> GetOverdueHoldingsAsync(Guid storeId, DateTime overdueBefore, CancellationToken ct = default);
        Task<List<SalesByProductReportRow>> GetSalesByProductAsync(Guid storeId, DateTime? dateFrom, DateTime? dateTo, CancellationToken ct = default);
        Task<int> DecreaseStockAsync(Guid stockId, int quantity, int reservedToRelease, Guid? userId, DateTime now, CancellationToken ct = default);
        Task<int> IncreaseOrderDetailBorrowedQuantityAsync(Guid orderDetailId, int quantity, Guid? userId, DateTime now, CancellationToken ct = default);
        Task<int> IncreaseHoldingQuantityAsync(Guid holdingId, int quantity, Guid? userId, DateTime now, CancellationToken ct = default);
        void AddOrderDetail(OrderDetail detail);
        void AddTechnician(Technician technician);
        void AddHolding(TechnicianHolding holding);
        void AddTransaction(HoldingTransaction transaction);
    }

    public class SalesByProductReportRow
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = default!;
        public string? ProductCode { get; set; }
        public int OrderedQuantity { get; set; }
        public int UsedBorrowedQuantity { get; set; }
        public int TotalSoldQuantity { get; set; }
    }
}
