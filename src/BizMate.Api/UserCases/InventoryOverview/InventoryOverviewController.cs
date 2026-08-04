using BizMate.Api.Serialization;
using BizMate.Application.Common.Responses;
using BizMate.Application.UserCases.InventoryOverview;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.InventoryOverview
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.Inventory)]
    [ApiController]
    [Authorize]
    public sealed class InventoryOverviewController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InventoryOverviewController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("overview")]
        [HasPermission(PermissionConstants.Product.View)]
        public async Task<IActionResult> GetOverview(
            [FromQuery] GetInventoryOverviewRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return ToJson(response);
        }

        private static IActionResult ToJson(BaseResponse response)
            => new JsonContentResult
            {
                StatusCode = response.Success ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest,
                Content = CommonJsonSerializer.SerializeObject(response)
            };
    }
}
