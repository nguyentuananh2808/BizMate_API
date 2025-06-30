using BizMate.Application.UserCases.InventoryReceipt.Commands.DeleteCreateInventoryReceipt;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.InventoryReceipt.DeleteInventoryReceipt
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.InventoryReceipt)]
    [ApiController]
    [Authorize]
    public class InventoryReceiptController : ControllerBase
    {
        private readonly DeleteInventoryReceiptPresenter _presenter;
        private readonly IMediator _mediator;

        public InventoryReceiptController(DeleteInventoryReceiptPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }


        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var request = new DeleteInventoryReceiptRequest(id);
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
