
using BizMate.Application.UserCases.DealerPrice.Commands.CreateDealerPrice;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.DealerPrice.CreateDealerPrice
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.DealerPrice)]
    [ApiController]
    [Authorize]
    public class DealerPriceController : ControllerBase
    {
        private readonly CreateDealerPricePresenter _presenter;
        private readonly IMediator _mediator;

        public DealerPriceController(CreateDealerPricePresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateDealerPriceRequest request)
        {
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }

}
