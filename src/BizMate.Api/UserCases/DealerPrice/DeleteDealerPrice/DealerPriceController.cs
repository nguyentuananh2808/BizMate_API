using BizMate.Api.UserCases.DealerPrice.DeleteDealerPrice;
using BizMate.Application.UserCases.DealerPrice.Commands.DeleteDealerPrice;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.DealerPrice.DeleteDealerPrice
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.DealerPrice)]
    [ApiController]
    [Authorize]
    public class DealerPriceController : ControllerBase
    {
        private readonly DeleteDealerPricePresenter _presenter;
        private readonly IMediator _mediator;

        public DealerPriceController(DeleteDealerPricePresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }


        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var request = new DeleteDealerPriceRequest(id);
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}