using BizMate.Application.UserCases.InventoryReceipt.Queries.InventoryReceipts;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.InventoryReceipt.GetInventoryReceipts
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.InventoryReceipt)]
    [ApiController]
    [Authorize]
    public class InventoryReceiptController : ControllerBase
    {
        private readonly GetInventoryReceiptsPresenter _presenter;
        private readonly IMediator _mediator;

        public InventoryReceiptController(GetInventoryReceiptsPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }

        [HttpPost("search")]
        public async Task<IActionResult> GetInventoryReceipts(GetInventoryReceiptsRequest request)
        {
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
