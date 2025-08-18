using BizMate.Application.UserCases.Customer.Commands.DeleteCustomer;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.Customer.DeleteCustomer
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.Customer)]
    [ApiController]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly DeleteCustomerPresenter _presenter;
        private readonly IMediator _mediator;

        public CustomerController(DeleteCustomerPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }


        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var request = new DeleteCustomerRequest(id);
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
