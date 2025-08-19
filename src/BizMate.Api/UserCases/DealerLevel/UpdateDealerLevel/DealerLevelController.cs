using BizMate.Application.UserCases.DealerLevel.Commands.UpdateDealerLevel;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.DealerLevel.UpdateDealerLevel
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.DealerLevel)]
    [ApiController]
    [Authorize]
    public class DealerLevelController : ControllerBase
    {
        private readonly UpdateDealerLevelPresenter _presenter;
        private readonly IMediator _mediator;

        public DealerLevelController(UpdateDealerLevelPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }


        [HttpPut]
        public async Task<IActionResult> Update(UpdateDealerLevelRequest request)
        {
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
