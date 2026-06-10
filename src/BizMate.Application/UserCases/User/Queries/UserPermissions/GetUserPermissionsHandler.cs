using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Responses;
using BizMate.Domain.Entities;
using MediatR;

namespace BizMate.Application.UserCases.User.Queries.UserPermissions
{
    public class GetUserPermissionsRequest : IRequest<GetUserPermissionsResponse>
    {
        public Guid UserId { get; set; }
        public Guid StoreId { get; set; }

        public GetUserPermissionsRequest(Guid userId, Guid storeId)
        {
            UserId = userId;
            StoreId = storeId;
        }
    }

    public class GetUserPermissionsResponse : BaseResponse
    {
        public UserPermissionsDto? Data { get; set; }

        public GetUserPermissionsResponse(UserPermissionsDto data) : base(true)
            => Data = data;

        public GetUserPermissionsResponse(bool success = false, string? message = null)
            : base(success, message) { }
    }

    public class UserPermissionsDto
    {
        public Guid UserId { get; set; }
        public Guid StoreId { get; set; }
        public List<UserRoleItemDto> Roles { get; set; } = new();
        public List<UserPermissionItemDto> DirectPermissions { get; set; } = new();
        public List<UserPermissionItemDto> RolePermissions { get; set; } = new();
        public List<UserPermissionItemDto> EffectivePermissions { get; set; } = new();
    }

    public class UserRoleItemDto
    {
        public Guid RoleId { get; set; }
        public string Name { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public bool IsSystem { get; set; }
    }

    public class UserPermissionItemDto
    {
        public Guid PermissionId { get; set; }
        public string Name { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public string Group { get; set; } = default!;
    }

    public class GetUserPermissionsHandler : IRequestHandler<GetUserPermissionsRequest, GetUserPermissionsResponse>
    {
        private readonly IUserRepository _userRepo;
        private readonly IUserRoleRepository _userRoleRepo;
        private readonly IUserPermissionRepository _userPermissionRepo;

        public GetUserPermissionsHandler(
            IUserRepository userRepo,
            IUserRoleRepository userRoleRepo,
            IUserPermissionRepository userPermissionRepo)
        {
            _userRepo = userRepo;
            _userRoleRepo = userRoleRepo;
            _userPermissionRepo = userPermissionRepo;
        }

        public async Task<GetUserPermissionsResponse> Handle(
            GetUserPermissionsRequest request,
            CancellationToken ct)
        {
            try
            {
                if (request.UserId == Guid.Empty)
                    return new GetUserPermissionsResponse(false, "UserId không hợp lệ.");

                var userExists = await _userRepo.ExistsInStoreAsync(request.UserId, request.StoreId, ct);
                if (!userExists)
                    return new GetUserPermissionsResponse(false, "Không tìm thấy nhân viên trong cửa hàng hiện tại.");

                var userRoles = await _userRoleRepo.GetByUserIdAndStoreIdWithPermissionsAsync(
                    request.UserId,
                    request.StoreId,
                    ct);
                var directUserPermissions = await _userPermissionRepo.GetByUserIdAndStoreIdAsync(
                    request.UserId,
                    request.StoreId,
                    ct);

                var roles = userRoles
                    .Where(ur => ur.Role is not null && !ur.Role.IsDeleted)
                    .Select(ur => new UserRoleItemDto
                    {
                        RoleId = ur.RoleId,
                        Name = ur.Role.Name,
                        DisplayName = ur.Role.DisplayName,
                        IsSystem = ur.Role.IsSystem
                    })
                    .OrderBy(r => r.Name)
                    .ToList();

                var rolePermissions = userRoles
                    .Where(ur => ur.Role is not null && !ur.Role.IsDeleted)
                    .SelectMany(ur => ur.Role.RolePermissions)
                    .Where(rp => !rp.IsDeleted && rp.Permission is not null && !rp.Permission.IsDeleted)
                    .Select(rp => ToPermissionItem(rp.Permission));

                var directPermissions = directUserPermissions
                    .Where(up => up.Permission is not null && !up.Permission.IsDeleted)
                    .Select(up => ToPermissionItem(up.Permission));

                var dto = new UserPermissionsDto
                {
                    UserId = request.UserId,
                    StoreId = request.StoreId,
                    Roles = roles,
                    DirectPermissions = DistinctAndSort(directPermissions),
                    RolePermissions = DistinctAndSort(rolePermissions),
                    EffectivePermissions = DistinctAndSort(rolePermissions.Concat(directPermissions))
                };

                return new GetUserPermissionsResponse(dto);
            }
            catch (Exception ex)
            {
                return new GetUserPermissionsResponse(false, ex.Message);
            }
        }

        private static UserPermissionItemDto ToPermissionItem(Permission permission)
            => new()
            {
                PermissionId = permission.Id,
                Name = permission.Name,
                DisplayName = permission.DisplayName,
                Group = permission.Group
            };

        private static List<UserPermissionItemDto> DistinctAndSort(IEnumerable<UserPermissionItemDto> permissions)
            => permissions
                .GroupBy(p => p.PermissionId)
                .Select(g => g.First())
                .OrderBy(p => p.Group)
                .ThenBy(p => p.Name)
                .ToList();
    }
}
