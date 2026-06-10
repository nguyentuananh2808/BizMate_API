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
        [HasPermission(PermissionConstants.User.View)]
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

        [HttpPost]
        [HasPermission(PermissionConstants.User.Create)]
        public async Task<IActionResult> CreateTechnician([FromBody] TechnicianBody? body, CancellationToken ct)
        {
            var response = await _mediator.Send(new CreateTechnicianRequest
            {
                Name = body?.Name ?? string.Empty,
                Phone = body?.Phone,
                ZaloPhone = body?.ZaloPhone,
                IsActive = body?.IsActive ?? true
            }, ct);

            return ToJson(response);
        }

        [HttpPut("{technicianId:guid}")]
        [HasPermission(PermissionConstants.User.Edit)]
        public async Task<IActionResult> UpdateTechnician(
            Guid technicianId,
            [FromBody] TechnicianBody? body,
            CancellationToken ct)
        {
            var response = await _mediator.Send(new UpdateTechnicianRequest
            {
                Id = technicianId,
                Name = body?.Name ?? string.Empty,
                Phone = body?.Phone,
                ZaloPhone = body?.ZaloPhone,
                IsActive = body?.IsActive ?? true
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

    public class TechnicianBody
    {
        public string Name { get; set; } = default!;
        public string? Phone { get; set; }
        public string? ZaloPhone { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
