using MediatR;
using System;

namespace BizMate.Application.UserCases.InventoryReceipt.Commands.DeleteCreateInventoryReceipt
{
    public class DeleteInventoryReceiptRequest : IRequest<DeleteInventoryReceiptResponse>
    {
        public Guid Id { get; set; }

        public DeleteInventoryReceiptRequest(Guid id)
        {
            Id = id;
        }
    }
}
