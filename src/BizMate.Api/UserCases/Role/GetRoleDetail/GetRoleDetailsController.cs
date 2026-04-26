using BizMate.Application.UserCases.RoleAggregate.Role.Queries.GetRoleDetail;
using BizMate.Domain.Constants;
using BizMate.Infrastructure.Security;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.Role.GetRoleDetail
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.Role)]
    [ApiController]
    public class GetRoleDetailsController : ControllerBase
    {
        private readonly GetRoleDetailsPresenter _presenter;
        private readonly IMediator _mediator;

        public GetRoleDetailsController(GetRoleDetailsPresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator  = mediator;
        }

        /// <summary>GET v1/role/{id} — Lấy chi tiết vai trò kèm danh sách permission</summary>
        [HttpGet("{id:guid}")]
        [HasPermission(PermissionConstants.Role.View)]
        public async Task<IActionResult> GetRoleDetail(Guid id, CancellationToken ct)
        {
            var response = await _mediator.Send(new GetRoleDetailRequest(id), ct);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
