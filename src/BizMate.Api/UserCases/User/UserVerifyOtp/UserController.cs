using BizMate.Api.UserCases.User.UserVerifyOtp;
using BizMate.Application.UserCases.User.Commands.UserVerifyOtp;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.User.UserVerifyOtp
{

    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.User)]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserVerifyOtpPresenter _presenter;
        private readonly IMediator _mediator;
        public UserController(UserVerifyOtpPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }

        [HttpPost(ApiNameConstants.Verify)]
        public async Task<IActionResult> Create(UserVerifyOtpRequest request)
        {
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
