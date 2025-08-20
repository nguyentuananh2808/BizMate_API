using BizMate.Application.UserCases.ImportReceipt.Queries.ImportReceipts;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.ImportReceipt.GetImportReceipts
{

    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.ImportReceipt)]
    [ApiController]
    [Authorize]
    public class ImportReceiptController : ControllerBase
    {
        private readonly GetImportReceiptsPresenter _presenter;
        private readonly IMediator _mediator;

        public ImportReceiptController(GetImportReceiptsPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }

        [HttpPost("search")]
        public async Task<IActionResult> GetImportReceipts(GetImportReceiptsRequest request)
        {
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
