using BizMate.Application.UserCases.ProductAggregate.ProductCategory.Commands.UpdateProductCategory;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.ProductAggregate.ProductCategory.UpdateProductCategory
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.ProductCategory)]
    [ApiController]
    [Authorize]
    public class ProductCategoryController : ControllerBase
    {
        private readonly UpdateProductCategoryPresenter _presenter;
        private readonly IMediator _mediator;

        public ProductCategoryController(UpdateProductCategoryPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }


        [HttpPut]
        public async Task<IActionResult> Update(UpdateProductCategoryRequest request)
        {
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
