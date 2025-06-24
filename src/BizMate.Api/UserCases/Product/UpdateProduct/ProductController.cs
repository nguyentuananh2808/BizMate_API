using BizMate.Application.UserCases.Product.Commands.UpdateProduct;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.Product.UpdateProduct
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.Product)]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly UpdateProductPresenter _presenter;
        private readonly IMediator _mediator;

        public ProductController(UpdateProductPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }


        [HttpPut]
        public async Task<IActionResult> Update(UpdateProductRequest request)
        {
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }

}
