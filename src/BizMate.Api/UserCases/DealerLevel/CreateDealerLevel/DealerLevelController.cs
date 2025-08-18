using BizMate.Application.UserCases.DealerLevel.Commands.CreateDealerLevel;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BizMate.Api.UserCases.DealerLevel.CreateDealerLevel
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.DealerLevel)]
    [ApiController]
    [Authorize]
    public class DealerLevelController : ControllerBase
    {
        private readonly CreateDealerLevelPresenter _presenter;
        private readonly IMediator _mediator;

        public DealerLevelController(CreateDealerLevelPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateDealerLevelRequest request)
        {
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }

}
