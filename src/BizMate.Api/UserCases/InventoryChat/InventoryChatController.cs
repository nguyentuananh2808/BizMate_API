using BizMate.Api.Serialization;
using BizMate.Application.Common.Responses;
using BizMate.Application.UserCases.InventoryChat;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.InventoryChat
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.InventoryChat)]
    [ApiController]
    [Authorize]
    public class InventoryChatController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InventoryChatController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("ask")]
        [HasPermission(PermissionConstants.Stock.View)]
        public async Task<IActionResult> Ask([FromBody] AskInventoryChatRequest request, CancellationToken cancellationToken)
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
