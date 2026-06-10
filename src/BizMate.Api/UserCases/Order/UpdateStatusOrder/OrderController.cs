using BizMate.Application.UserCases.Order.Commands.UpdateStatusOrder;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.Order.UpdateStatusOrder
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.Order)]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly UpdateStatusOrderPresenter _presenter;
        private readonly IMediator _mediator;
        private readonly IAuthorizationService _authorizationService;

        public OrderController(
            UpdateStatusOrderPresenter presenter,
            IMediator mediator,
            IAuthorizationService authorizationService)
        {
            _presenter = presenter;
            _mediator = mediator;
            _authorizationService = authorizationService;
        }


        [HttpPut("update_status")]
        public async Task<IActionResult> Update(UpdateStatusOrderRequest request)
        {
            var requiredPermission = ResolveRequiredPermission(request.StatusCode);
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
                "CANCELLED" => PermissionConstants.Order.Cancel,
                "APPROVED" or "PACKED" or "COMPLETED" => PermissionConstants.Order.Approve,
                _ => PermissionConstants.Order.Edit
            };
        }
    }
}
