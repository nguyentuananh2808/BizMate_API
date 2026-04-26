using BizMate.Application.UserCases.RoleAggregate.Role.Commands.DeleteRole;
using BizMate.Domain.Constants;
using BizMate.Infrastructure.Security;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.Role.DeleteRole
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.Role)]
    [ApiController]
    public class DeleteRolesController : ControllerBase
    {
        private readonly DeleteRolesPresenter _presenter;
        private readonly IMediator _mediator;

        public DeleteRolesController(DeleteRolesPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator  = mediator;
        }

        /// <summary>
        /// Xóa vai trò (không xóa được role hệ thống hoặc đang được gán cho nhân viên).
        /// </summary>
        [HttpDelete("{id:guid}")]
        [HasPermission(PermissionConstants.Role.Delete)]
        public async Task<IActionResult> DeleteRole(Guid id, CancellationToken ct)
        {
            var response = await _mediator.Send(new DeleteRoleRequest(id), ct);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
