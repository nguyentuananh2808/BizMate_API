using BizMate.Application.UserCases.InventoryReceipt.Commands.UpdateInventoryReceipt;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.InventoryReceipt.UpdateInventoryReceipt
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.InventoryReceipt)]
    [ApiController]
    [Authorize]
    public class InventoryReceiptController : ControllerBase
    {
        private readonly UpdateInventoryReceiptPresenter _presenter;
        private readonly IMediator _mediator;

        public InventoryReceiptController(UpdateInventoryReceiptPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }


        [HttpPut]
        public async Task<IActionResult> Update(UpdateInventoryReceiptRequest request)
        {
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
