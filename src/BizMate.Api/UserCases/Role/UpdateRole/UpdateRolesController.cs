using BizMate.Application.UserCases.RoleAggregate.Role.Commands.UpdateRole;
using BizMate.Domain.Constants;
using BizMate.Infrastructure.Security;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.Role.UpdateRole
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.Role)]
    [ApiController]
    public class UpdateRoleController : ControllerBase
    {
        private readonly UpdateRolePresenter _presenter;
        private readonly IMediator _mediator;

        public UpdateRoleController(UpdateRolePresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }

        /// <summary>
        /// PUT v1/role/{id}
        /// Cập nhật tên hiển thị và danh sách permission của vai trò.
        /// Body: { "displayName": "Quản lý kho", "permissionIds": ["guid1","guid2",...] }
        /// </summary>
        [HttpPut("{id:guid}")]
        [HasPermission(PermissionConstants.Role.Edit)]
        public async Task<IActionResult> UpdateRole(
            Guid id,
            [FromBody] UpdateRoleRequestBody body,
            CancellationToken ct)
        {
            var request = new UpdateRoleRequest
            {
                Id = id,
                DisplayName = body.DisplayName,
                PermissionIds = body.PermissionIds,
            };

            var response = await _mediator.Send(request, ct);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }

}
