using BizMate.Application.UserCases.Order.Queries.GetOrder;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.Order.GetOrder
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.Order)]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly GetOrderPresenter _presenter;
        private readonly IMediator _mediator;

        public OrderController(GetOrderPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetOrder(Guid id)
        {
            var request = new GetOrderRequest(id);
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
