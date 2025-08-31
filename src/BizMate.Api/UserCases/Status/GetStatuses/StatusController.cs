using BizMate.Application.UserCases.Status.Queries.GetStatuses;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.Status.GetStatuses
{

    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.Status)]
    [ApiController]
    [Authorize]
    public class StatusController : ControllerBase
    {
        private readonly GetStatusesPresenter _presenter;
        private readonly IMediator _mediator;

        public StatusController(GetStatusesPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }

        [HttpPost("GetByGroup")]
        public async Task<IActionResult> GetStatuss(GetStatusesRequest request)
        {
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
