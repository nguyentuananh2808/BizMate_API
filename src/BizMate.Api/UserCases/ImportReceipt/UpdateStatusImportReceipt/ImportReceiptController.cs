using BizMate.Api.UserCases.StatusImportReceipt.UpdateStatusStatusImportReceipt;
using BizMate.Application.UserCases.ImportReceipt.Commands.UpdateStatusImportReceipt;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.ImportReceipt.UpdateStatusImportReceipt
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.ImportReceipt)]
    [ApiController]
    [Authorize]
    public class ImportReceiptController : ControllerBase
    {
        private readonly UpdateStatusImportReceiptPresenter _presenter;
        private readonly IMediator _mediator;

        public ImportReceiptController(UpdateStatusImportReceiptPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }


        [HttpPut("update_status")]
        public async Task<IActionResult> Update(UpdateStatusImportReceiptRequest request)
        {
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
