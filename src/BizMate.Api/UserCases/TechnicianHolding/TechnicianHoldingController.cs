using BizMate.Api.Serialization;
using BizMate.Application.Common.Responses;
using BizMate.Application.UserCases.TechnicianHolding;
using BizMate.Domain.Constants;
using BizMate.Domain.Entities;
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
            var response = await _mediator.Send(new ReturnTechnicianHoldingByTypeRequest
            {
                TechnicianId = body?.TechnicianId ?? Guid.Empty,
                Items = body?.Items?.Select(x => new ReturnTechnicianHoldingItem
                {
                    ProductId = x.ProductId,
                    BorrowType = x.BorrowType,
                    Quantity = x.Quantity
                }).ToList() ?? new List<ReturnTechnicianHoldingItem>()
            }, ct);

            return ToJson(response);
        }

        [HttpGet("requests")]
        [HasPermission(PermissionConstants.Stock.View)]
        public async Task<IActionResult> GetBorrowRequests(
            [FromQuery] TechnicianBorrowRequestStatus? status,
            [FromQuery] Guid? technicianId,
            CancellationToken ct)
        {
            var response = await _mediator.Send(new GetTechnicianBorrowRequestsRequest
            {
                Status = status,
                TechnicianId = technicianId
            }, ct);

            return ToJson(response);
        }

        [HttpPost("requests")]
        [HasPermission(PermissionConstants.Stock.View)]
        public async Task<IActionResult> CreateBorrowRequest([FromBody] CreateTechnicianBorrowBody? body, CancellationToken ct)
        {
            var response = await _mediator.Send(new CreateTechnicianBorrowRequest
            {
                TechnicianId = body?.TechnicianId ?? Guid.Empty,
                BorrowType = body?.BorrowType ?? TechnicianBorrowType.Daily,
                NeededDate = body?.NeededDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
                Description = body?.Description,
                Items = body?.Items?.Select(x => new TechnicianBorrowRequestItemInput
                {
                    ProductId = x.ProductId,
                    Quantity = x.Quantity
                }).ToList() ?? new List<TechnicianBorrowRequestItemInput>()
            }, ct);

            return ToJson(response);
        }

        [HttpPost("requests/{requestId:guid}/approve")]
        [HasPermission(PermissionConstants.Stock.Adjust)]
        public async Task<IActionResult> ApproveBorrowRequest(Guid requestId, CancellationToken ct)
        {
            var response = await _mediator.Send(new ApproveTechnicianBorrowRequest
            {
                RequestId = requestId
            }, ct);

            return ToJson(response);
        }

        [HttpPost("requests/{requestId:guid}/reject")]
        [HasPermission(PermissionConstants.Stock.Adjust)]
        public async Task<IActionResult> RejectBorrowRequest(
            Guid requestId,
            [FromBody] RejectTechnicianBorrowBody? body,
            CancellationToken ct)
        {
            var response = await _mediator.Send(new RejectTechnicianBorrowRequest
            {
                RequestId = requestId,
                Reason = body?.Reason
            }, ct);

            return ToJson(response);
        }

        [HttpPost("use")]
        [HasPermission(PermissionConstants.Stock.Adjust)]
        public async Task<IActionResult> UseHolding([FromBody] UseTechnicianHoldingBody? body, CancellationToken ct)
        {
            var response = await _mediator.Send(new UseTechnicianHoldingRequest
            {
                TechnicianId = body?.TechnicianId ?? Guid.Empty,
                ProductId = body?.ProductId ?? Guid.Empty,
                BorrowType = body?.BorrowType ?? TechnicianBorrowType.Assigned,
                Quantity = body?.Quantity ?? 0,
                Note = body?.Note
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
        public TechnicianBorrowType BorrowType { get; set; } = TechnicianBorrowType.Assigned;
        public int Quantity { get; set; }
    }

    public class CreateTechnicianBorrowBody
    {
        public Guid TechnicianId { get; set; }
        public TechnicianBorrowType BorrowType { get; set; } = TechnicianBorrowType.Daily;
        public DateOnly NeededDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        public string? Description { get; set; }
        public List<CreateTechnicianBorrowItemBody> Items { get; set; } = new();
    }

    public class CreateTechnicianBorrowItemBody
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class RejectTechnicianBorrowBody
    {
        public string? Reason { get; set; }
    }

    public class UseTechnicianHoldingBody
    {
        public Guid TechnicianId { get; set; }
        public Guid ProductId { get; set; }
        public TechnicianBorrowType BorrowType { get; set; } = TechnicianBorrowType.Assigned;
        public int Quantity { get; set; }
        public string? Note { get; set; }
    }
}
