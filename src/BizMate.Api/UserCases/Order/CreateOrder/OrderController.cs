using BizMate.Application.UserCases.Order.Commands.CreateOrder;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.Order.CreateOrder
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.Order)]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly CreateOrderPresenter _presenter;
        private readonly IMediator _mediator;

        public OrderController(CreateOrderPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderRequest request)
        {
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
