using BizMate.Application.UserCases.ProductAggregate.Product.Queries.Products;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.ProductAggregate.Product.GetProducts
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.Product)]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly GetProductsPresenter _presenter;
        private readonly IMediator _mediator;

        public ProductController(GetProductsPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }

        [HttpPost("search")]
        public async Task<IActionResult> GetProducts(GetProductsRequest request)
        {
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
