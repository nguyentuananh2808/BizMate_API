using BizMate.Application.UserCases.Customer.Queries.Customers;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.Customer.GetCustomers
{

    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.Customer)]
    [ApiController]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly GetCustomersPresenter _presenter;
        private readonly IMediator _mediator;

        public CustomerController(GetCustomersPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }

        [HttpPost("search")]
        public async Task<IActionResult> GetCustomers(GetCustomersRequest request)
        {
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
