//using BizMate.Application.Common.Dto.CoreDto;
//using BizMate.Application.Common.Responses;
//using System.Text.Json.Serialization;

//namespace BizMate.Application.UserCases.InventoryReceipt.Queries.InventoryReceipts
//{
//    public class GetInventoryReceiptsResponse : BaseResponse
//    {
//        public IEnumerable<InventoryReceiptCoreDto> InventoryReceipts { get; }
//        public int TotalCount { get; }

//        [JsonConstructor]
//        public GetInventoryReceiptsResponse(IEnumerable<InventoryReceiptCoreDto> inventoryReceipts, int totalCount, bool success = true) : base(success)
//        {
//            InventoryReceipts = inventoryReceipts;
//            TotalCount = totalCount;
//        }

//        public GetInventoryReceiptsResponse(bool success = false, string? message = null) : base(success, message)
//        {
//        }
//    }
//}
