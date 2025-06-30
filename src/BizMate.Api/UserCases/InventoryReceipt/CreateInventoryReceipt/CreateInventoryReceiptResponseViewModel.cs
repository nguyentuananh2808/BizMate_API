using BizMate.Application.UserCases.InventoryReceipt.Commands.CreateInventoryReceipt;

namespace BizMate.Api.UserCases.InventoryReceipt.CreateInventoryReceipt
{
    public class CreateInventoryReceiptResponseViewModel
    {
        public Guid Id { get; set; }
        public string InventoryCode { get; set; } = default!;
        public DateTime Date { get; set; } = DateTime.UtcNow;

        public int Type { get; set; }

        public Guid StoreId { get; set; }
        public string StoreName { get; set; } = default!;

        public Guid CreatedByUserId { get; set; }
        public string CreatedByUserName { get; set; } = default!;

        public string? SupplierName { get; set; }      // If Import
        public string? CustomerName { get; set; }      // If Export
        public string? CustomerPhone { get; set; }
        public string? DeliveryAddress { get; set; }

        public string? Description { get; set; }
        public IEnumerable<InventoryReceiptDetailDto> InventoryDetails { get; set; }

        public CreateInventoryReceiptResponseViewModel(CreateInventoryReceiptResponse response)
        {
            Id = response.Id;
            InventoryCode = response.InventoryCode;
            Date = response.Date;
            Type = response.Type;

            StoreId = response.StoreId;
            StoreName = response.StoreName;

            CreatedByUserId = response.CreatedByUserId;
            CreatedByUserName = response.CreatedByUserName;

            SupplierName = response.SupplierName;
            CustomerName = response.CustomerName;
            CustomerPhone = response.CustomerPhone;
            DeliveryAddress = response.DeliveryAddress;
            Description = response.Description;

            InventoryDetails = response.InventoryDetails;
        }

    }
}
