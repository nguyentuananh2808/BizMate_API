using MediatR;

namespace BizMate.Application.UserCases.InventoryReceipt.Queries.InventoryReceipt
{
    public class GetInventoryReceiptRequest : IRequest<GetInventoryReceiptResponse>
    {
        public Guid Id { get; set; }

        public GetInventoryReceiptRequest(Guid id)
        {
            Id = id;
        }
    }
}
