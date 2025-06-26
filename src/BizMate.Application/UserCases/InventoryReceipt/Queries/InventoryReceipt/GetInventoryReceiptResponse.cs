using BizMate.Application.Common.Responses;
using BizMate.Application.UserCases.InventoryReceipt.Commands.CreateInventoryReceipt;

namespace BizMate.Application.UserCases.InventoryReceipt.Queries.InventoryReceipt
{
    public class GetInventoryReceiptResponse : BaseResponse
    {
        public string InventoryCode { get; set; } = default!;
        public DateTime Date { get; set; }
        public int Type { get; set; }
        public Guid StoreId { get; set; }
        public string StoreName { get; set; } = default!;
        public Guid CreatedByUserId { get; set; }
        public string CreatedByUserName { get; set; } = default!;
        public string? SupplierName { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? Description { get; set; }
        public IEnumerable<InventoryReceiptDetailDto> InventoryDetails { get; set; }

        public GetInventoryReceiptResponse(bool success = true, string? message = null) : base(success, message)
        {
        }
    }
}
