using BizMate.Application.UserCases.DealerPrice.Commands.UpdateDealerPrice;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.DealerPrice.UpdateDealerPrice
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.DealerPrice)]
    [ApiController]
    [Authorize]
    public class DealerPriceController : ControllerBase
    {
        private readonly UpdateDealerPricePresenter _presenter;
        private readonly IMediator _mediator;

        public DealerPriceController(UpdateDealerPricePresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }


        [HttpPut]
        public async Task<IActionResult> Update(UpdateDealerPriceRequest request)
        {
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
