using BizMate.Api.UserCases.InventoryReceipt.CreateInventoryReceipt;
using BizMate.Application.UserCases.InventoryReceipt.Commands.CreateInventoryReceipt;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.InventoryReceipt.CreateInventoryReceipt
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.InventoryReceipt)]
    [ApiController]
    [Authorize]
    public class InventoryReceiptController : ControllerBase
    {
        private readonly CreateInventoryReceiptPresenter _presenter;
        private readonly IMediator _mediator;

        public InventoryReceiptController(CreateInventoryReceiptPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateInventoryReceiptRequest request)
        {
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
