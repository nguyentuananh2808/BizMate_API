using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Responses;
using MediatR;

namespace BizMate.Application.UserCases.RoleAggregate.Role.Queries.GetRoles
{
    // ══════════════════════════════════════════════════════════════════════════
    // GET ROLES (danh sách)
    // ══════════════════════════════════════════════════════════════════════════
    public class GetRolesRequest : IRequest<GetRolesResponse> { }

    public class GetRolesResponse : BaseResponse
    {
        public List<RoleListItemDto> Roles { get; set; } = new();

        public GetRolesResponse(List<RoleListItemDto> roles) : base(true)
            => Roles = roles;

        public GetRolesResponse(bool success = false, string? message = null)
            : base(success, message) { }
    }

    public class RoleListItemDto
    {
        public Guid   Id              { get; set; }
        public string Name            { get; set; } = default!;
        public string DisplayName     { get; set; } = default!;
        public bool   IsSystem        { get; set; }
        public int    PermissionCount { get; set; }
        public int    UserCount       { get; set; }
    }

    public class GetRolesHandler : IRequestHandler<GetRolesRequest, GetRolesResponse>
    {
        private readonly IRoleRepository _roleRepo;
        public GetRolesHandler(IRoleRepository roleRepo) => _roleRepo = roleRepo;

        public async Task<GetRolesResponse> Handle(
            GetRolesRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var roles = await _roleRepo.GetAllWithCountsAsync(cancellationToken);
                var dtos = roles.Select(r => new RoleListItemDto
                {
                    Id              = r.Id,
                    Name            = r.Name,
                    DisplayName     = r.DisplayName,
                    IsSystem        = r.IsSystem,
                    PermissionCount = r.RolePermissions.Count,
                    UserCount       = r.UserRoles.Count(ur => !ur.IsDeleted)
                }).ToList();

                return new GetRolesResponse(dtos);
            }
            catch (Exception ex)
            {
                return new GetRolesResponse(false, ex.Message);
            }
        }
    }
}

namespace BizMate.Application.UserCases.RoleAggregate.Role.Queries.GetRoleDetail
{
    // ══════════════════════════════════════════════════════════════════════════
    // GET ROLE DETAIL (kèm danh sách permission)
    // ══════════════════════════════════════════════════════════════════════════
    public class GetRoleDetailRequest : IRequest<GetRoleDetailResponse>
    {
        public Guid Id { get; set; }
        public GetRoleDetailRequest(Guid id) => Id = id;
    }

    public class GetRoleDetailResponse : BaseResponse
    {
        public RoleDetailDto? Role { get; set; }

        public GetRoleDetailResponse(RoleDetailDto role) : base(true) => Role = role;
        public GetRoleDetailResponse(bool success = false, string? message = null)
            : base(success, message) { }
    }

    public class RoleDetailDto
    {
        public Guid   Id          { get; set; }
        public string Name        { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public bool   IsSystem    { get; set; }
        public List<RolePermissionItemDto> Permissions { get; set; } = new();
    }

    public class RolePermissionItemDto
    {
        public Guid   PermissionId { get; set; }
        public string Name         { get; set; } = default!;
        public string DisplayName  { get; set; } = default!;
        public string Group        { get; set; } = default!;
    }

    public class GetRoleDetailHandler : IRequestHandler<GetRoleDetailRequest, GetRoleDetailResponse>
    {
        private readonly IRoleRepository _roleRepo;
        public GetRoleDetailHandler(IRoleRepository roleRepo) => _roleRepo = roleRepo;

        public async Task<GetRoleDetailResponse> Handle(
            GetRoleDetailRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var role = await _roleRepo.GetByIdWithPermissionsAsync(request.Id, cancellationToken);
                if (role is null)
                    return new GetRoleDetailResponse(false, "Không tìm thấy vai trò.");

                var dto = new RoleDetailDto
                {
                    Id          = role.Id,
                    Name        = role.Name,
                    DisplayName = role.DisplayName,
                    IsSystem    = role.IsSystem,
                    Permissions = role.RolePermissions.Select(rp => new RolePermissionItemDto
                    {
                        PermissionId = rp.PermissionId,
                        Name         = rp.Permission.Name,
                        DisplayName  = rp.Permission.DisplayName,
                        Group        = rp.Permission.Group
                    }).ToList()
                };

                return new GetRoleDetailResponse(dto);
            }
            catch (Exception ex)
            {
                return new GetRoleDetailResponse(false, ex.Message);
            }
        }
    }
}
