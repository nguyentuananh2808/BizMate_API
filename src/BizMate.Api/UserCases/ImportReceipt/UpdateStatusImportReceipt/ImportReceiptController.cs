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
        private readonly IAuthorizationService _authorizationService;

        public ImportReceiptController(
            UpdateStatusImportReceiptPresenter presenter,
            IMediator mediator,
            IAuthorizationService authorizationService)
        {
            _presenter = presenter;
            _mediator = mediator;
            _authorizationService = authorizationService;
        }


        [HttpPut("update_status")]
        public async Task<IActionResult> Update(UpdateStatusImportReceiptRequest request)
        {
            var requiredPermission = ResolveRequiredPermission(request.CodeStatus);
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, requiredPermission);
            if (!authorizationResult.Succeeded)
                return Forbid();

            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }

        private static string ResolveRequiredPermission(string? statusCode)
        {
            return statusCode?.Trim().ToUpperInvariant() switch
            {
                "CANCELLED" => PermissionConstants.ImportReceipt.Cancel,
                _ => PermissionConstants.ImportReceipt.Edit
            };
        }
    }
}
