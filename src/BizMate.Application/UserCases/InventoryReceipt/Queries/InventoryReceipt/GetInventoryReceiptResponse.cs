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

        public GetInventoryReceiptResponse(
                string inventoryCode,
                DateTime date,
                int type,
                Guid storeId,
                string storeName,
                Guid createdByUserId,
                string createdByUserName,
                string? supplierName,
                string? customerName,
                string? customerPhone,
                string? deliveryAddress,
                string? description,
                IEnumerable<InventoryReceiptDetailDto> inventoryDetails,
                bool success = true,
                string? message = null) : base(success, message)
        {
            InventoryCode = inventoryCode;
            Date = date;
            Type = type;
            StoreId = storeId;
            StoreName = storeName;
            CreatedByUserId = createdByUserId;
            CreatedByUserName = createdByUserName;
            SupplierName = supplierName;
            CustomerName = customerName;
            CustomerPhone = customerPhone;
            DeliveryAddress = deliveryAddress;
            Description = description;
            InventoryDetails = inventoryDetails;
        }
        public GetInventoryReceiptResponse(bool success = false,
                string? message = null) : base(success, message)
        { }
    }
}
