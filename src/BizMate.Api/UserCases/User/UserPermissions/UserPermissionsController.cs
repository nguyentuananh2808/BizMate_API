using BizMate.Application.Common.Security;
using BizMate.Application.UserCases.User.Commands.UserPermissions;
using BizMate.Application.UserCases.User.Queries.UserPermissions;
using BizMate.Domain.Constants;
using BizMate.Infrastructure.Security;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.User.UserPermissions
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.User)]
    [ApiController]
    public class UserPermissionsController : ControllerBase
    {
        private readonly GetUserPermissionsPresenter _getPresenter;
        private readonly UserPermissionMutationPresenter _mutationPresenter;
        private readonly IMediator _mediator;
        private readonly IUserSession _userSession;

        public UserPermissionsController(
            GetUserPermissionsPresenter getPresenter,
            UserPermissionMutationPresenter mutationPresenter,
            IMediator mediator,
            IUserSession userSession)
        {
            _getPresenter = getPresenter;
            _mutationPresenter = mutationPresenter;
            _mediator = mediator;
            _userSession = userSession;
        }

        /// <summary>
        /// GET v1/user/{userId}/permissions
        /// Gets direct, role, and effective permissions for a user in the current store.
        /// </summary>
        [HttpGet("{userId:guid}/permissions")]
        [HasPermission(PermissionConstants.Role.View)]
        public async Task<IActionResult> GetUserPermissions(Guid userId, CancellationToken ct)
        {
            var response = await _mediator.Send(
                new GetUserPermissionsRequest(userId, _userSession.StoreId),
                ct);
            _getPresenter.Handle(response);
            return _getPresenter.ContentResult;
        }

        /// <summary>
        /// POST v1/user/{userId}/permissions
        /// Adds one or more direct permissions for a user in the current store.
        /// Body: { "permissionIds": ["guid1", "guid2"] }
        /// </summary>
        [HttpPost("{userId:guid}/permissions")]
        [HasPermission(PermissionConstants.Role.Assign)]
        public async Task<IActionResult> AddUserPermissions(
            Guid userId,
            [FromBody] UserPermissionsBody? body,
            CancellationToken ct)
        {
            var response = await _mediator.Send(new AddUserPermissionsRequest
            {
                UserId = userId,
                StoreId = _userSession.StoreId,
                PermissionIds = body?.PermissionIds ?? new List<Guid>()
            }, ct);

            _mutationPresenter.Handle(response);
            return _mutationPresenter.ContentResult;
        }

        /// <summary>
        /// PUT v1/user/{userId}/permissions
        /// Replaces all direct permissions for a user in the current store.
        /// Body: { "permissionIds": ["guid1", "guid2"] }
        /// </summary>
        [HttpPut("{userId:guid}/permissions")]
        [HasPermission(PermissionConstants.Role.Assign)]
        public async Task<IActionResult> ReplaceUserPermissions(
            Guid userId,
            [FromBody] UserPermissionsBody? body,
            CancellationToken ct)
        {
            var response = await _mediator.Send(new ReplaceUserPermissionsRequest
            {
                UserId = userId,
                StoreId = _userSession.StoreId,
                PermissionIds = body?.PermissionIds ?? new List<Guid>()
            }, ct);

            _mutationPresenter.Handle(response);
            return _mutationPresenter.ContentResult;
        }

        /// <summary>
        /// DELETE v1/user/{userId}/permissions/{permissionId}
        /// Removes one direct permission from a user in the current store.
        /// </summary>
        [HttpDelete("{userId:guid}/permissions/{permissionId:guid}")]
        [HasPermission(PermissionConstants.Role.Assign)]
        public async Task<IActionResult> RemoveUserPermission(
            Guid userId,
            Guid permissionId,
            CancellationToken ct)
        {
            var response = await _mediator.Send(new RemoveUserPermissionRequest
            {
                UserId = userId,
                StoreId = _userSession.StoreId,
                PermissionId = permissionId
            }, ct);

            _mutationPresenter.Handle(response);
            return _mutationPresenter.ContentResult;
        }

        /// <summary>
        /// DELETE v1/user/{userId}/permissions
        /// Clears all direct permissions from a user in the current store.
        /// </summary>
        [HttpDelete("{userId:guid}/permissions")]
        [HasPermission(PermissionConstants.Role.Assign)]
        public async Task<IActionResult> ClearUserPermissions(Guid userId, CancellationToken ct)
        {
            var response = await _mediator.Send(new ClearUserPermissionsRequest
            {
                UserId = userId,
                StoreId = _userSession.StoreId
            }, ct);

            _mutationPresenter.Handle(response);
            return _mutationPresenter.ContentResult;
        }
    }
}
