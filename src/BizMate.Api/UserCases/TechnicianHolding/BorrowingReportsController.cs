using BizMate.Api.Serialization;
using BizMate.Application.Common.Responses;
using BizMate.Application.UserCases.TechnicianHolding;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.TechnicianHolding
{
    [Route(ApiNameConstants.ApiV1 + "report")]
    [ApiController]
    public class BorrowingReportsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BorrowingReportsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("sales-by-product")]
        [HasPermission(PermissionConstants.Report.View)]
        public async Task<IActionResult> GetSalesByProduct(
            [FromQuery] DateTime? dateFrom,
            [FromQuery] DateTime? dateTo,
            CancellationToken ct)
        {
            var response = await _mediator.Send(new GetSalesByProductReportRequest
            {
                DateFrom = dateFrom,
                DateTo = dateTo
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
