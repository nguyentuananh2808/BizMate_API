using BizMate.Application.UserCases.InventoryReceipt.Queries.InventoryReceipt;

namespace BizMate.Api.UserCases.InventoryReceipt.GetInventoryReceipt
{
    public class GetInventoryReceiptResponseViewModel
    {
        public GetInventoryReceiptResponse InventoryReceipt { get; set; }
        public GetInventoryReceiptResponseViewModel(GetInventoryReceiptResponse inventoryReceipt)
        {
            InventoryReceipt = inventoryReceipt;
        }
    }
}
