using BizMate.Application.UserCases.ProductAggregate.ProductCategory.Queries.ProductCategories;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.ProductAggregate.ProductCategory.GetProductCategories
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.ProductCategory)]
    [ApiController]
    [Authorize]
    public class ProductCategoryController : ControllerBase
    {
        private readonly GetProductCategoriesPresenter _presenter;
        private readonly IMediator _mediator;

        public ProductCategoryController(GetProductCategoriesPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetProductCategories(GetProductCategoriesRequest request)
        {
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
