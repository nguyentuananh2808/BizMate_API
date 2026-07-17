using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Requests;
using BizMate.Application.Common.Responses;
using BizMate.Application.Common.Security;
using BizMate.Application.Common.Users;
using MediatR;

namespace BizMate.Application.UserCases.User.Queries.UserManagement
{
    public class GetUsersRequest : SearchCore, IRequest<GetUsersResponse>
    {
        public bool? IsActive { get; set; }
    }

    public class GetUsersResponse : BaseResponse
    {
        public List<UserListItemDto> Users { get; set; } = new();
        public int TotalCount { get; set; }

        public GetUsersResponse(List<UserListItemDto> users, int totalCount) : base(true)
        {
            Users = users;
            TotalCount = totalCount;
        }

        public GetUsersResponse(bool success = false, string? message = null)
            : base(success, message) { }
    }

    public class UserListItemDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Role { get; set; } = default!;
        public Guid StoreId { get; set; }
        public string StoreName { get; set; } = default!;
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int RoleCount { get; set; }
        public int DirectPermissionCount { get; set; }
    }

    public class GetUsersHandler : IRequestHandler<GetUsersRequest, GetUsersResponse>
    {
        private readonly IUserRepository _userRepo;
        private readonly IUserSession _userSession;

        public GetUsersHandler(IUserRepository userRepo, IUserSession userSession)
        {
            _userRepo = userRepo;
            _userSession = userSession;
        }

        public async Task<GetUsersResponse> Handle(GetUsersRequest request, CancellationToken ct)
        {
            try
            {
                var pageSize = request.PageSize <= 0 ? 20 : request.PageSize;
                var pageIndex = request.PageIndex < 0 ? 0 : request.PageIndex;

                var (users, totalCount) = await _userRepo.SearchUsersWithPagingAsync(
                    _userSession.StoreId,
                    request.KeySearch,
                    pageIndex,
                    pageSize,
                    request.IsActive,
                    ct);

                var dtos = users.Select(u => new UserListItemDto
                {
                    Id = u.Id,
                    Code = u.Code,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = UserRoleNameResolver.Resolve(u),
                    StoreId = u.StoreId,
                    StoreName = u.Store?.Name ?? string.Empty,
                    IsActive = u.IsActive,
                    CreatedDate = u.CreatedDate,
                    RoleCount = u.UserRoles.Count(ur => !ur.IsDeleted),
                    DirectPermissionCount = u.UserPermissions.Count(up => !up.IsDeleted)
                }).ToList();

                return new GetUsersResponse(dtos, totalCount);
            }
            catch
            {
                return new GetUsersResponse(false, "Không thể tải danh sách nhân viên. Vui lòng thử lại.");
            }
        }
    }
}
