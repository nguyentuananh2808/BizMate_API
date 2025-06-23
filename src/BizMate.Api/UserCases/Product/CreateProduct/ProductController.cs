using BizMate.Application.UserCases.Product.Commands.CreateProduct;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.Product.CreateProduct
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.Product)]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly ProductPresenter _presenter;
        private readonly IMediator _mediator;

        public ProductController(ProductPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateProductRequest request)
        {
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
