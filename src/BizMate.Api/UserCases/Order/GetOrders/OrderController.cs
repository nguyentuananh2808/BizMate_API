using BizMate.Application.UserCases.Order.Queries.GetOrders;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.Order.GetOrders
{

    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.Order)]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly GetOrdersPresenter _presenter;
        private readonly IMediator _mediator;

        public OrderController(GetOrdersPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }

        [HttpPost("search")]
        public async Task<IActionResult> GetOrders(GetOrdersRequest request)
        {
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
