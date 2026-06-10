using BizMate.Application.UserCases.RoleAggregate.Permission.Queries.GetPermissions;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.Permission.GetPermissions
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.Permission)]
    [ApiController]
    public class GetPermissionsController : ControllerBase
    {
        private readonly GetPermissionsPresenter _presenter;
        private readonly IMediator _mediator;

        public GetPermissionsController(GetPermissionsPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }

        /// <summary>
        /// GET v1/permission
        /// Trả về toàn bộ danh sách permission nhóm theo Group.
        /// FE dùng endpoint này để render checkbox khi tạo/sửa vai trò.
        /// </summary>
        [HttpGet]
        [HasPermission(PermissionConstants.Role.View)]
        public async Task<IActionResult> GetPermissions(CancellationToken ct)
        {
            var response = await _mediator.Send(new GetPermissionsRequest(), ct);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
