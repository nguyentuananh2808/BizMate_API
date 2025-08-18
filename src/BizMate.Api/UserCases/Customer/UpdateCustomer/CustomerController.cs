using BizMate.Application.UserCases.Customer.Commands.UpdateCustomer;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.Customer.UpdateCustomer
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.Customer)]
    [ApiController]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly UpdateCustomerPresenter _presenter;
        private readonly IMediator _mediator;

        public CustomerController(UpdateCustomerPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }


        [HttpPut]
        public async Task<IActionResult> Update(UpdateCustomerRequest request)
        {
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
