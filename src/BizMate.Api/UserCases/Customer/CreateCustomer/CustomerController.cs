using BizMate.Application.UserCases.CustomerAggregate.Customer.Commands.CreateCustomer;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.Customer.CreateCustomer
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.Customer)]
    [ApiController]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly CreateCustomerPresenter _presenter;
        private readonly IMediator _mediator;

        public CustomerController(CreateCustomerPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateCustomerRequest request)
        {
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
