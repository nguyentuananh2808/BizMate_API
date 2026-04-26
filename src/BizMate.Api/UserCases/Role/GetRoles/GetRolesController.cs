using BizMate.Application.UserCases.RoleAggregate.Role.Queries.GetRoles;
using BizMate.Domain.Constants;
using BizMate.Infrastructure.Security;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.Role.GetRoles
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.Role)]
    [ApiController]
    public class GetRolesController : ControllerBase
    {
        private readonly GetRolesPresenter _presenter;
        private readonly IMediator _mediator;

        public GetRolesController(GetRolesPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }

        /// <summary>GET v1/role — Lấy danh sách tất cả vai trò</summary>
        [HttpGet]
        [HasPermission(PermissionConstants.Role.View)]
        public async Task<IActionResult> GetRoles(CancellationToken ct)
        {
            var response = await _mediator.Send(new GetRolesRequest(), ct);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
