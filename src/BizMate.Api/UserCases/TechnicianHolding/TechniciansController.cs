using BizMate.Api.Serialization;
using BizMate.Application.Common.Responses;
using BizMate.Application.UserCases.TechnicianHolding;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.TechnicianHolding
{
    [Route(ApiNameConstants.ApiV1 + "technician")]
    [ApiController]
    public class TechniciansController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TechniciansController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [HasPermission(PermissionConstants.Stock.View)]
        public async Task<IActionResult> GetTechnicians(
            [FromQuery] string? keyword,
            [FromQuery] bool includeInactive,
            CancellationToken ct)
        {
            var response = await _mediator.Send(new GetTechniciansRequest
            {
                Keyword = keyword,
                IncludeInactive = includeInactive
            }, ct);

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
