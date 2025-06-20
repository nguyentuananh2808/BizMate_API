using BizMate.Application.UserCases.User.Commands.UserRegister;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.User.UserRegister
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.User)]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserRegisterPresenter _presenter;
        private readonly IMediator _mediator;
        public UserController(UserRegisterPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }

        [HttpPost(ApiNameConstants.UserRegister)]
        public async Task<IActionResult> Create(UserRegisterRequest request)
        {
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
