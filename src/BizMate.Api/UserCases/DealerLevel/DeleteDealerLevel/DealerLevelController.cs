using BizMate.Application.UserCases.DealerLevel.Commands.DeleteDealerLevel;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.DealerLevel.DeleteDealerLevel
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.DealerLevel)]
    [ApiController]
    [Authorize]
    public class DealerLevelController : ControllerBase
    {
        private readonly DeleteDealerLevelPresenter _presenter;
        private readonly IMediator _mediator;

        public DealerLevelController(DeleteDealerLevelPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }


        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var request = new DeleteDealerLevelRequest(id);
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
