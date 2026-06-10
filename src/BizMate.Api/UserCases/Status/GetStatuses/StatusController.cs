using BizMate.Application.UserCases.Status.Queries.GetStatuses;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.Status.GetStatuses
{

    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.Status)]
    [ApiController]
    [Authorize]
    public class StatusController : ControllerBase
    {
        private readonly GetStatusesPresenter _presenter;
        private readonly IMediator _mediator;
        private readonly IAuthorizationService _authorizationService;

        public StatusController(
            GetStatusesPresenter presenter,
            IMediator mediator,
            IAuthorizationService authorizationService)
        {
            _presenter = presenter;
            _mediator = mediator;
            _authorizationService = authorizationService;
        }

        [HttpPost("GetByGroup")]
        public async Task<IActionResult> GetStatuss(GetStatusesRequest request)
        {
            var requiredPermission = ResolveRequiredPermission(request.Group);
            if (requiredPermission is null)
                return Forbid();

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, requiredPermission);
            if (!authorizationResult.Succeeded)
                return Forbid();

            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }

        private static string? ResolveRequiredPermission(string? group)
        {
            return group?.Trim().ToUpperInvariant() switch
            {
                "ORDER" => PermissionConstants.Order.View,
                "IMPORTRECEIPT" => PermissionConstants.ImportReceipt.View,
                "EXPORTRECEIPT" => PermissionConstants.ExportReceipt.View,
                "WARRANTY" => PermissionConstants.Product.View,
                _ => null
            };
        }
    }
}
