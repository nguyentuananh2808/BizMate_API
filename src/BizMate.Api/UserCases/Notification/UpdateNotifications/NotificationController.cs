using BizMate.Application.UserCases.Notification.Commands.UpdateNotifications;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.Notification.UpdateNotifications
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.Notification)]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly UpdateNotificationsPresenter _presenter;
        private readonly IMediator _mediator;

        public NotificationController(UpdateNotificationsPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }


        [HttpPut]
        public async Task<IActionResult> Update(UpdateNotificationsRequest request)
        {
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
