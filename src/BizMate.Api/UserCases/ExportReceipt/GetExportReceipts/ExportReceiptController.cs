using BizMate.Application.UserCases.ExportReceipt.Queries.GetExportReceipts;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.ExportReceipt.GetExportReceipts
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.ExportReceipt)]
    [ApiController]
    [Authorize]
    public class ExportReceiptController : ControllerBase
    {
        private readonly GetExportReceiptsPresenter _presenter;
        private readonly IMediator _mediator;

        public ExportReceiptController(GetExportReceiptsPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }

        [HttpPost("search")]
        public async Task<IActionResult> GetExportReceipts(GetExportReceiptsRequest request)
        {
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
