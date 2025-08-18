using BizMate.Application.UserCases.DealerLevel.Queries.DealerLevel;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.DealerLevel.GetDealerLevel
{

    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.DealerLevel)]
    [ApiController]
    [Authorize]
    public class DealerLevelController : ControllerBase
    {
        private readonly GetDealerLevelPresenter _presenter;
        private readonly IMediator _mediator;

        public DealerLevelController(GetDealerLevelPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetDealerLevel(Guid id)
        {
            var request = new GetDealerLevelRequest(id);
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}

