using BizMate.Application.UserCases.InventoryReceipt.Commands.CreateInventoryReceipt;

namespace BizMate.Api.UserCases.InventoryReceipt.CreateInventoryReceipt
{
    public class CreateInventoryReceiptResponseViewModel
    {
        public string InventoryCode { get; set; }
        public DateTime Date { get; set; }
        public int Type { get; set; }
        public string StoreName { get; set; }
        public string CreatedByUserName { get; set; }
        public string? SupplierName { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? Description { get; set; }
        public IEnumerable<InventoryReceiptDetailDto> InventoryDetails { get; set; }

        public CreateInventoryReceiptResponseViewModel(CreateInventoryReceiptResponse response)
        {
            InventoryCode = response.InventoryCode;
            Date = response.Date;
            Type = response.Type;
            StoreName = response.StoreName;
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
