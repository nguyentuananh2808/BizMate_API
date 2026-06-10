using BizMate.Api.Serialization;
using BizMate.Application.Common.Responses;
using BizMate.Application.UserCases.TechnicianHolding;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.TechnicianHolding
{
    [Route(ApiNameConstants.ApiV1 + "technician-holdings")]
    [ApiController]
    public class TechnicianHoldingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TechnicianHoldingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [HasPermission(PermissionConstants.Stock.View)]
        public async Task<IActionResult> GetHoldings([FromQuery] Guid? technicianId, CancellationToken ct)
        {
            var response = await _mediator.Send(new GetTechnicianHoldingsRequest
            {
                TechnicianId = technicianId
            }, ct);

            return ToJson(response);
        }

        [HttpGet("overdue")]
        [HasPermission(PermissionConstants.Stock.View)]
        public async Task<IActionResult> GetOverdueHoldings(CancellationToken ct)
        {
            var response = await _mediator.Send(new GetOverdueHoldingsRequest(), ct);
            return ToJson(response);
        }

        [HttpPost("return")]
        [HasPermission(PermissionConstants.Stock.Adjust)]
        public async Task<IActionResult> ReturnItems([FromBody] ReturnTechnicianHoldingBody? body, CancellationToken ct)
        {
            var response = await _mediator.Send(new ReturnTechnicianHoldingRequest
            {
                TechnicianId = body?.TechnicianId ?? Guid.Empty,
                Items = body?.Items?.Select(x => new ReturnTechnicianHoldingItem
                {
                    ProductId = x.ProductId,
                    Quantity = x.Quantity
                }).ToList() ?? new List<ReturnTechnicianHoldingItem>()
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

    public class ReturnTechnicianHoldingBody
    {
        public Guid TechnicianId { get; set; }
        public List<ReturnTechnicianHoldingItemBody> Items { get; set; } = new();
    }

    public class ReturnTechnicianHoldingItemBody
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
