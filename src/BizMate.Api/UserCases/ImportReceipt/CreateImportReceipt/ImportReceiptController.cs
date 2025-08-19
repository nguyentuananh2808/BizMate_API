using BizMate.Application.UserCases.ImportReceipt.Commands.CreateImportReceipt;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.ImportReceipt.CreateImportReceipt
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.ImportReceipt)]
    [ApiController]
    [Authorize]
    public class ImportReceiptController : ControllerBase
    {
        private readonly CreateImportReceiptPresenter _presenter;
        private readonly IMediator _mediator;

        public ImportReceiptController(CreateImportReceiptPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateImportReceiptRequest request)
        {
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
