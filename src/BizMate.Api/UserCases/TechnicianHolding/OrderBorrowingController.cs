using BizMate.Api.Serialization;
using BizMate.Application.Common.Responses;
using BizMate.Application.UserCases.TechnicianHolding;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.TechnicianHolding
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.Order)]
    [ApiController]
    public class OrderBorrowingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrderBorrowingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("{orderId:guid}/export")]
        [HasPermission(PermissionConstants.Stock.Adjust)]
        public async Task<IActionResult> ExportForTechnician(
            Guid orderId,
            [FromBody] ExportOrderForTechnicianBody? body,
            CancellationToken ct)
        {
            var response = await _mediator.Send(new ExportOrderForTechnicianRequest
            {
                OrderId = orderId,
                TechnicianId = body?.TechnicianId,
                Items = body?.Items?.Select(x => new ExportOrderForTechnicianItem
                {
                    ProductId = x.ProductId,
                    OrderedQuantity = x.OrderedQuantity,
                    BorrowedQuantity = x.BorrowedQuantity
                }).ToList() ?? new List<ExportOrderForTechnicianItem>(),
                TechnicianExports = body?.TechnicianExports?.Select(x => new TechnicianExportRequest
                {
                    TechnicianId = x.TechnicianId,
                    Items = x.Items?.Select(i => new ExportOrderForTechnicianItem
                    {
                        ProductId = i.ProductId,
                        OrderedQuantity = i.OrderedQuantity,
                        BorrowedQuantity = i.BorrowedQuantity
                    }).ToList() ?? new List<ExportOrderForTechnicianItem>()
                }).ToList() ?? new List<TechnicianExportRequest>()
            }, ct);

            return ToJson(response);
        }

        [HttpPost("{orderId:guid}/use-borrowed")]
        [HasPermission(PermissionConstants.Order.Edit)]
        public async Task<IActionResult> UseBorrowed(
            Guid orderId,
            [FromBody] UseBorrowedItemBody? body,
            CancellationToken ct)
        {
            var response = await _mediator.Send(new UseBorrowedItemRequest
            {
                OrderId = orderId,
                TechnicianId = body?.TechnicianId,
                ProductId = body?.ProductId ?? Guid.Empty,
                Quantity = body?.Quantity ?? 0
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

    public class ExportOrderForTechnicianBody
    {
        public Guid? TechnicianId { get; set; }
        public List<ExportOrderForTechnicianItemBody> Items { get; set; } = new();
        public List<TechnicianExportBody> TechnicianExports { get; set; } = new();
    }

    public class TechnicianExportBody
    {
        public Guid TechnicianId { get; set; }
        public List<ExportOrderForTechnicianItemBody> Items { get; set; } = new();
    }

    public class ExportOrderForTechnicianItemBody
    {
        public Guid ProductId { get; set; }
        public int OrderedQuantity { get; set; }
        public int BorrowedQuantity { get; set; }
    }

    public class UseBorrowedItemBody
    {
        public Guid? TechnicianId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
