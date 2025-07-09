using BizMate.Application.UserCases.ProductAggregate.ProductCategory.Queries.ProductCategory;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.ProductAggregate.ProductCategory.GetProductCategory
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.ProductCategory)]
    [ApiController]
    [Authorize]
    public class ProductCategoryController : ControllerBase
    {
        private readonly GetProductCategoryPresenter _presenter;
        private readonly IMediator _mediator;

        public ProductCategoryController(GetProductCategoryPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetProductCategory(Guid id)
        {
            var request = new GetProductCategoryRequest(id);
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
