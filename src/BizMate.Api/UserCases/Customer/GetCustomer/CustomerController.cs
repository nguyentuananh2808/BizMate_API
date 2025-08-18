using BizMate.Application.UserCases.Customer.Queries.Customer;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.Customer.GetCustomer
{

    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.Customer)]
    [ApiController]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly GetCustomerPresenter _presenter;
        private readonly IMediator _mediator;

        public CustomerController(GetCustomerPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCustomer(Guid id)
        {
            var request = new GetCustomerRequest(id);
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
