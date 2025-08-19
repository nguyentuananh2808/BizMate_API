using BizMate.Application.UserCases.ImportReceipt.Commands.UpdateImportReceipt;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.ImportReceipt.UpdateImportReceipt
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.ImportReceipt)]
    [ApiController]
    [Authorize]
    public class ImportReceiptController : ControllerBase
    {
        private readonly UpdateImportReceiptPresenter _presenter;
        private readonly IMediator _mediator;

        public ImportReceiptController(UpdateImportReceiptPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }


        [HttpPut]
        public async Task<IActionResult> Update(UpdateImportReceiptRequest request)
        {
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
