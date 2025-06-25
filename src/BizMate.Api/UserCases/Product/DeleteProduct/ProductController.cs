using BizMate.Application.UserCases.Product.Commands.DeleteProduct;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.Product.DeleteProduct
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.Product)]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly DeleteProductPresenter _presenter;
        private readonly IMediator _mediator;

        public ProductController(DeleteProductPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }


        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var request = new DeleteProductRequest(id);
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
