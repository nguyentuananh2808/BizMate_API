using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Responses;
using MediatR;

namespace BizMate.Application.UserCases.RoleAggregate.Permission.Queries.GetPermissions
{
    public class GetPermissionsRequest : IRequest<GetPermissionsResponse> { }

    public class GetPermissionsResponse : BaseResponse
    {
        public List<PermissionGroupDto> Groups { get; set; } = new();

        public GetPermissionsResponse(List<PermissionGroupDto> groups) : base(true)
            => Groups = groups;

        public GetPermissionsResponse(bool success = false, string? message = null)
            : base(success, message) { }
    }

    public class PermissionGroupDto
    {
        public string Group { get; set; } = default!;
        public List<PermissionItemDto> Permissions { get; set; } = new();
    }

    public class PermissionItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
    }

    public class GetPermissionsHandler : IRequestHandler<GetPermissionsRequest, GetPermissionsResponse>
    {
        private readonly IPermissionRepository _permRepo;

        public GetPermissionsHandler(IPermissionRepository permRepo)
            => _permRepo = permRepo;

        /// <summary>
        /// Gets all active permissions and groups them for the permission management screen.
        /// </summary>
        public async Task<GetPermissionsResponse> Handle(
            GetPermissionsRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var perms = await _permRepo.GetAllAsync(cancellationToken);

                var groups = perms
                    .GroupBy(p => p.Group)
                    .Select(g => new PermissionGroupDto
                    {
                        Group = g.Key,
                        Permissions = g.Select(p => new PermissionItemDto
                        {
                            Id = p.Id,
                            Name = p.Name,
                            DisplayName = p.DisplayName
                        }).ToList()
                    })
                    .ToList();

                return new GetPermissionsResponse(groups);
            }
            catch
            {
                return new GetPermissionsResponse(
                    false,
                    "Không thể tải danh sách quyền. Vui lòng thử lại.");
            }
        }
    }
}
