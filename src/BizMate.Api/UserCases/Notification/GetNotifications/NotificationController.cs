using BizMate.Application.UserCases.Notification.Queries.GetNotifications;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BizMate.Api.UserCases.Notification.GetNotifications
{

    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.Notification)]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly GetNotificationsPresenter _presenter;
        private readonly IMediator _mediator;

        public NotificationController(GetNotificationsPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }

        [HttpPost("get-notification")]
        public async Task<IActionResult> GetNotifications(GetNotificationsRequest request)
        {
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
