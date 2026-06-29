using BizMate.Application.UserCases.User.Commands.UserManagement;
using BizMate.Application.UserCases.User.Queries.UserManagement;
using BizMate.Domain.Constants;
using BizMate.Infrastructure.Security;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.User.UserManagement
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.User)]
    [ApiController]
    public class UserManagementController : ControllerBase
    {
        private readonly GetUsersPresenter _getUsersPresenter;
        private readonly GetUserDetailPresenter _getDetailPresenter;
        private readonly UserMutationPresenter _mutationPresenter;
        private readonly IMediator _mediator;

        public UserManagementController(
            GetUsersPresenter getUsersPresenter,
            GetUserDetailPresenter getDetailPresenter,
            UserMutationPresenter mutationPresenter,
            IMediator mediator)
        {
            _getUsersPresenter = getUsersPresenter;
            _getDetailPresenter = getDetailPresenter;
            _mutationPresenter = mutationPresenter;
            _mediator = mediator;
        }

        /// <summary>
        /// POST v1/user/search
        /// Gets store users so the UI can select a user before assigning permissions.
        /// </summary>
        [HttpPost("search")]
        [HasPermission(PermissionConstants.User.View)]
        public async Task<IActionResult> GetUsers([FromBody] GetUsersRequest? request, CancellationToken ct)
        {
            var response = await _mediator.Send(request ?? new GetUsersRequest(), ct);
            _getUsersPresenter.Handle(response);
            return _getUsersPresenter.ContentResult;
        }

        /// <summary>
        /// GET v1/user/{userId}
        /// Gets a store user detail.
        /// </summary>
        [HttpGet("{userId:guid}")]
        [HasPermission(PermissionConstants.User.View)]
        public async Task<IActionResult> GetUserDetail(Guid userId, CancellationToken ct)
        {
            var response = await _mediator.Send(new GetUserDetailRequest(userId), ct);
            _getDetailPresenter.Handle(response);
            return _getDetailPresenter.ContentResult;
        }

        /// <summary>
        /// POST v1/user
        /// Creates a user in the current store.
        /// </summary>
        [HttpPost]
        [HasPermission(PermissionConstants.User.Create)]
        public async Task<IActionResult> CreateUser([FromBody] CreateStoreUserBody? body, CancellationToken ct)
        {
            var response = await _mediator.Send(new CreateStoreUserRequest
            {
                FullName = body?.FullName ?? string.Empty,
                Email = body?.Email ?? string.Empty,
                Password = body?.Password ?? string.Empty,
                Phone = body?.Phone,
                RoleId = body?.RoleId ?? Guid.Empty,
                IsActive = body?.IsActive ?? true
            }, ct);

            _mutationPresenter.Handle(response);
            return _mutationPresenter.ContentResult;
        }

        /// <summary>
        /// PUT v1/user/{userId}
        /// Updates a user in the current store.
        /// </summary>
        [HttpPut("{userId:guid}")]
        [HasPermission(PermissionConstants.User.Edit)]
        public async Task<IActionResult> UpdateUser(
            Guid userId,
            [FromBody] UpdateStoreUserBody? body,
            CancellationToken ct)
        {
            var response = await _mediator.Send(new UpdateStoreUserRequest
            {
                UserId = userId,
                FullName = body?.FullName ?? string.Empty,
                Email = body?.Email ?? string.Empty,
                Phone = body?.Phone,
                RoleId = body?.RoleId,
                IsActive = body?.IsActive ?? false
            }, ct);

            _mutationPresenter.Handle(response);
            return _mutationPresenter.ContentResult;
        }

        /// <summary>
        /// DELETE v1/user/{userId}
        /// Soft deletes a user and revokes store roles and direct permissions.
        /// </summary>
        [HttpDelete("{userId:guid}")]
        [HasPermission(PermissionConstants.User.Delete)]
        public async Task<IActionResult> DeleteUser(Guid userId, CancellationToken ct)
        {
            var response = await _mediator.Send(new DeleteStoreUserRequest
            {
                UserId = userId
            }, ct);

            _mutationPresenter.Handle(response);
            return _mutationPresenter.ContentResult;
        }
    }
}
