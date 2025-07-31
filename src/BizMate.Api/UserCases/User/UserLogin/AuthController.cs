using BizMate.Application.UserCases.User.Queries.UserLogin;
using BizMate.Domain.Constants; 
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.User.UserLogin
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.Authentication)]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserLoginPresenter _presenter;
        private readonly IMediator _mediator;

        public AuthController(UserLoginPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }

        [HttpPost(ApiNameConstants.Login)]
        public async Task<IActionResult> Login(UserLoginRequest request)
        {
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
