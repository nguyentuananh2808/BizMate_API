using BizMate.Application.UserCases.Order.Commands.UpdateStatusOrder;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.Order.UpdateStatusOrder
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.Order)]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly UpdateStatusOrderPresenter _presenter;
        private readonly IMediator _mediator;

        public OrderController(UpdateStatusOrderPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }


        [HttpPut("update_status")]
        public async Task<IActionResult> Update(UpdateStatusOrderRequest request)
        {
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
