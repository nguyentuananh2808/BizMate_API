using BizMate.Api.Serialization;
using BizMate.Application.Common.Responses;
using BizMate.Application.UserCases.ProductAggregate.CategoryGroup;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BizMate.Api.UserCases.ProductAggregate.CategoryGroup
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.CategoryGroup)]
    [ApiController]
    [Authorize]
    public class CategoryGroupController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoryGroupController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("search")]
        [HasPermission(PermissionConstants.ProductCategory.View)]
        public async Task<IActionResult> Search(GetCategoryGroupsRequest request)
        {
            return ToActionResult(await _mediator.Send(request));
        }

        [HttpGet("GetAll")]
        [HasPermission(PermissionConstants.ProductCategory.View)]
        public async Task<IActionResult> GetAll()
        {
            return ToActionResult(await _mediator.Send(new GetAllCategoryGroupsRequest()));
        }

        [HttpGet("{id:guid}")]
        [HasPermission(PermissionConstants.ProductCategory.View)]
        public async Task<IActionResult> GetById(Guid id)
        {
            return ToActionResult(await _mediator.Send(new GetCategoryGroupRequest(id)));
        }

        [HttpPost]
        [HasPermission(PermissionConstants.ProductCategory.Create)]
        public async Task<IActionResult> Create(CreateCategoryGroupRequest request)
        {
            return ToActionResult(await _mediator.Send(request));
        }

        [HttpPut]
        [HasPermission(PermissionConstants.ProductCategory.Edit)]
        public async Task<IActionResult> Update(UpdateCategoryGroupRequest request)
        {
            return ToActionResult(await _mediator.Send(request));
        }

        [HttpPut("{id:guid}/categories")]
        [HasPermission(PermissionConstants.ProductCategory.Edit)]
        public async Task<IActionResult> AssignCategories(Guid id, AssignCategoryGroupCategoriesRequest request)
        {
            request.Id = id;
            return ToActionResult(await _mediator.Send(request));
        }

        [HttpDelete("{id:guid}")]
        [HasPermission(PermissionConstants.ProductCategory.Delete)]
        public async Task<IActionResult> Delete(Guid id)
        {
            return ToActionResult(await _mediator.Send(new DeleteCategoryGroupRequest(id)));
        }

        private IActionResult ToActionResult(BaseResponse response)
        {
            return new JsonContentResult
            {
                StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest),
                Content = CommonJsonSerializer.SerializeObject(response)
            };
        }
    }
}
