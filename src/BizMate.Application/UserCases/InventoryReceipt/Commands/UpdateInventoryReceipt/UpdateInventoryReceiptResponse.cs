using BizMate.Application.Common.Responses;
using BizMate.Application.UserCases.InventoryReceipt.Commands.CreateInventoryReceipt;

namespace BizMate.Application.UserCases.InventoryReceipt.Commands.UpdateInventoryReceipt
{
    public class UpdateInventoryReceiptResponse : BaseResponse  
    {
        public Guid Id { get; set; }
        public string InventoryCode { get; set; } = default!;
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public Guid RowVersion { get; set; }


        /// <summary>
        /// 1 = Import (Phiếu nhập), 2 = Export (Phiếu xuất)
        /// </summary>
        public int Type { get; set; }

        // Store info
        public Guid StoreId { get; set; }
        public string StoreName { get; set; } = default!;

        // Creator info
        public Guid CreatedByUserId { get; set; }
        public string CreatedByUserName { get; set; } = default!;

        // Optional info based on Type
        public string? SupplierName { get; set; }      // If Import
        public string? CustomerName { get; set; }      // If Export
        public string? CustomerPhone { get; set; }
        public string? DeliveryAddress { get; set; }

        public string? Description { get; set; }
        public Guid? StatusId { get; set; }
        public string? StatusName { get; set; }
        public string? StatusCode { get; set; }

        // Receipt Details
        public IEnumerable<InventoryReceiptDetailDto> InventoryDetails { get; set; } = new List<InventoryReceiptDetailDto>();

        public UpdateInventoryReceiptResponse(bool success = true, string? message = null) : base(success, message)
        {
        }
    }
}
