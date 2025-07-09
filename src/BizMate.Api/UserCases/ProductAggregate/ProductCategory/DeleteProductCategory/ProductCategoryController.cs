using BizMate.Application.UserCases.ProductAggregate.ProductCategory.Commands.DeleteProductCategory;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.ProductAggregate.ProductCategory.DeleteProductCategory
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.ProductCategory)]
    [ApiController]
    [Authorize]
    public class ProductCategoryController : ControllerBase
    {
        private readonly DeleteProductCategoryPresenter _presenter;
        private readonly IMediator _mediator;

        public ProductCategoryController(DeleteProductCategoryPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }


        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var request = new DeleteProductCategoryRequest(id);
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
