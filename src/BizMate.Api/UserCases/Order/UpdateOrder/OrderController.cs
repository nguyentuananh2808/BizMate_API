using BizMate.Application.UserCases.Order.Commands.UpdateOrder;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.Order.UpdateOrder
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.Order)]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly UpdateOrderPresenter _presenter;
        private readonly IMediator _mediator;

        public OrderController(UpdateOrderPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }


        [HttpPut]
        public async Task<IActionResult> Update(UpdateOrderRequest request)
        {
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
