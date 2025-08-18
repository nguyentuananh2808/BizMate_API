using BizMate.Application.UserCases.DealerLevel.Queries.DealerLevels;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.DealerLevel.GetDealerLevels
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.DealerLevel)]
    [ApiController]
    [Authorize]
    public class DealerLevelController : ControllerBase
    {
        private readonly GetDealerLevelsPresenter _presenter;
        private readonly IMediator _mediator;

        public DealerLevelController(GetDealerLevelsPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }

        [HttpPost("search")]
        public async Task<IActionResult> GetDealerLevels(GetDealerLevelsRequest request)
        {
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}