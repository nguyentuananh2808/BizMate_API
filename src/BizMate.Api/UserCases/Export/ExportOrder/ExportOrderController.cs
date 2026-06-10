using BizMate.Application.UserCases.Export.Queries.ExportOrders;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.Export.ExportOrder
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.ExportOrder)]
    [ApiController]
    [Authorize]
    public class ExportOrderController : ControllerBase
    {
        private readonly ExportOrdersPresenter _presenter;
        private readonly IMediator _mediator;

        public ExportOrderController(ExportOrdersPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }

        [HttpPost("export-order")]
        [HasPermission(PermissionConstants.Report.Export)]
        public async Task<IActionResult> ExportOrders(ExportOrdersRequest request)
        {
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
