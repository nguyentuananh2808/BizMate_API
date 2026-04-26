using BizMate.Application.UserCases.ProductAggregate.ProductItem.Queries.GetProductItems;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.ProductAggregate.ProductItem.GetProductItems
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.ProductItem)]
    [ApiController]
    [Authorize]
    public class ProductItemController : ControllerBase
    {
        private readonly GetProductItemsPresenter _presenter;
        private readonly IMediator _mediator;

        public ProductItemController(GetProductItemsPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductItems(
            [FromQuery] Guid productId,
            [FromQuery] int? status,
            [FromQuery] string? keyword,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20)
        {
            var request = new GetProductItemsRequest
            {
                ProductId = productId,
                Status = status,
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
