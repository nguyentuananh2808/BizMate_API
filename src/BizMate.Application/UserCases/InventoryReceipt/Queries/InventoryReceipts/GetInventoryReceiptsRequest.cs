using BizMate.Application.Common.Requests;
using MediatR;

namespace BizMate.Application.UserCases.InventoryReceipt.Queries.InventoryReceipts
{
    public class GetInventoryReceiptsRequest : SearchCore, IRequest<GetInventoryReceiptsResponse>
    {
        public int? Type { get; set; } // 1: Nhập, 2: Xuất
    }
}
