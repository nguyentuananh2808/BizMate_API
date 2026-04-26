using BizMate.Application.UserCases.RoleAggregate.Role.Commands.CreateRole;
using BizMate.Domain.Constants;
using BizMate.Infrastructure.Security;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.Role.CreateRole
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.Role)]
    [ApiController]
    public class CreateRolesController : ControllerBase
    {
        private readonly CreateRolesPresenter _presenter;
        private readonly IMediator _mediator;

        public CreateRolesController(CreateRolesPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator  = mediator;
        }

        /// <summary>
        /// Tạo vai trò mới với danh sách permission.
        /// Body: { "name": "Kế toán", "displayName": "Kế toán", "permissionIds": ["guid1","guid2",...] }
        /// </summary>
        [HttpPost]
        [HasPermission(PermissionConstants.Role.Create)]
        public async Task<IActionResult> CreateRole(
            [FromBody] CreateRoleRequest request, CancellationToken ct)
        {
            var response = await _mediator.Send(request, ct);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
