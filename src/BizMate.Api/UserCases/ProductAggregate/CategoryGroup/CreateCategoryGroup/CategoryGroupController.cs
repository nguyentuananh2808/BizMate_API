//using BizMate.Api.UserCases.ProductAggregate.Product.CreateProduct;
//using BizMate.Application.UserCases.ProductAggregate.CategoryGroup;
//using BizMate.Domain.Constants;
//using MediatR;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//namespace BizMate.Api.UserCases.ProductAggregate.CategoryGroup.CreateCategoryGroup
//{

//    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.ProductCategory)]
//    [ApiController]
//    [Authorize]
//    public class ProductController : ControllerBase
//    {
//        private readonly CreateProductPresenter _presenter;
//        private readonly IMediator _mediator;

//        public ProductController(CreateProductPresenter presenter, IMediator mediator)
//        {
//            _presenter = presenter;
//            _mediator = mediator;
//        }


//        [HttpPost]
//        [HasPermission(PermissionConstants.CategoryGroup.Create)]
//        public async Task<IActionResult> Create(CreateCategoryGroupRequest request)
//        {
//            var response = await _mediator.Send(request);
//            _presenter.Handle(response);
//            return _presenter.ContentResult;
//        }
//    }
//}
