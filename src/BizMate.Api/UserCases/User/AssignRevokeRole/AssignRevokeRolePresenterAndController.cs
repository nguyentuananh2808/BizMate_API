using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Security;
using BizMate.Application.UserCases.User.Commands.AssignRole;
using BizMate.Application.UserCases.User.Commands.RevokeRole;
using BizMate.Domain.Constants;
using BizMate.Infrastructure.Security;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BizMate.Api.UserCases.User.AssignRole
{
    // ── ResponseViewModel ─────────────────────────────────────────────────────
    public class AssignRoleResponseViewModel
    {
        public bool   Success { get; set; }
        public string Message { get; set; } = default!;

        public AssignRoleResponseViewModel(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }

    // ── Presenter ─────────────────────────────────────────────────────────────
    public class AssignRolePresenter : IOutputPort<AssignRoleResponse>
    {
        public JsonContentResult ContentResult { get; } = new();

        public void Handle(AssignRoleResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
            ContentResult.Content = CommonJsonSerializer.SerializeObject(
                new AssignRoleResponseViewModel(response.Success, response.Message ?? string.Empty));
        }
    }

    // ── Controller ────────────────────────────────────────────────────────────
    [Route("v1/user")]
    [ApiController]
    public class AssignRoleController : ControllerBase
    {
        private readonly AssignRolePresenter _presenter;
        private readonly IMediator _mediator;
        private readonly IUserSession _userSession;

        public AssignRoleController(
            AssignRolePresenter presenter,
            IMediator mediator,
            IUserSession userSession)
        {
            _presenter   = presenter;
            _mediator    = mediator;
            _userSession = userSession;
        }

        /// <summary>
        /// POST v1/user/{userId}/role
        /// Gán một vai trò cho nhân viên trong store hiện tại.
        /// Body: { "roleId": "guid" }
        /// </summary>
        [HttpPost("{userId:guid}/role")]
        [HasPermission(PermissionConstants.Role.Assign)]
        public async Task<IActionResult> AssignRole(
            Guid userId,
            [FromBody] AssignRoleBody body,
            CancellationToken ct)
        {
            var request = new AssignRoleRequest
            {
                UserId  = userId,
                RoleId  = body.RoleId,
                StoreId = _userSession.StoreId,  // lấy từ JWT claim "store_id"
            };

            var response = await _mediator.Send(request, ct);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }

    public class AssignRoleBody
    {
        public Guid RoleId { get; set; }
    }
}

namespace BizMate.Api.UserCases.User.RevokeRole
{
    using BizMate.Application.UserCases.User.Commands.RevokeRole;

    // ── ResponseViewModel ─────────────────────────────────────────────────────
    public class RevokeRoleResponseViewModel
    {
        public bool   Success { get; set; }
        public string Message { get; set; } = default!;

        public RevokeRoleResponseViewModel(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }

    // ── Presenter ─────────────────────────────────────────────────────────────
    public class RevokeRolePresenter : IOutputPort<RevokeRoleResponse>
    {
        public JsonContentResult ContentResult { get; } = new();

        public void Handle(RevokeRoleResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
            ContentResult.Content = CommonJsonSerializer.SerializeObject(
                new RevokeRoleResponseViewModel(response.Success, response.Message ?? string.Empty));
        }
    }

    // ── Controller ────────────────────────────────────────────────────────────
    [Route("v1/user")]
    [ApiController]
    public class RevokeRoleController : ControllerBase
    {
        private readonly RevokeRolePresenter _presenter;
        private readonly IMediator _mediator;
        private readonly IUserSession _userSession;

        public RevokeRoleController(
            RevokeRolePresenter presenter,
            IMediator mediator,
            IUserSession userSession)
        {
            _presenter   = presenter;
            _mediator    = mediator;
            _userSession = userSession;
        }

        /// <summary>
        /// DELETE v1/user/{userId}/role/{roleId}
        /// Thu hồi một vai trò khỏi nhân viên trong store hiện tại.
        /// </summary>
        [HttpDelete("{userId:guid}/role/{roleId:guid}")]
        [HasPermission(PermissionConstants.Role.Assign)]
        public async Task<IActionResult> RevokeRole(
            Guid userId,
            Guid roleId,
            CancellationToken ct)
        {
            var request = new RevokeRoleRequest(userId, roleId, _userSession.StoreId);
            var response = await _mediator.Send(request, ct);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
