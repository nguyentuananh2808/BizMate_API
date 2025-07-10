using BizMate.Application.UserCases.ProductAggregate.ProductCategory.Commands.CreateProductCategory;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.ProductAggregate.ProductCategory.CreateProductCategory
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.ProductCategory)]
    [ApiController]
    [Authorize]

    public class ProductCategoryController
    {
        private readonly CreateProductCategoryPresenter _presenter;
        private readonly IMediator _mediator;

        public ProductCategoryController(CreateProductCategoryPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }

        /// <summary>
        /// create product category
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductCategoryRequest request)
        {
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
