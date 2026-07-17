using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Responses;
using BizMate.Application.Common.Security;
using BizMate.Application.Common.Users;
using MediatR;

namespace BizMate.Application.UserCases.User.Queries.UserManagement
{
    public class GetUserDetailRequest : IRequest<GetUserDetailResponse>
    {
        public Guid UserId { get; set; }

        public GetUserDetailRequest(Guid userId)
        {
            UserId = userId;
        }
    }

    public class GetUserDetailResponse : BaseResponse
    {
        public UserDetailDto? User { get; set; }

        public GetUserDetailResponse(UserDetailDto user) : base(true)
            => User = user;

        public GetUserDetailResponse(bool success = false, string? message = null)
            : base(success, message) { }
    }

    public class UserDetailDto : UserListItemDto
    {
        public DateTime? UpdatedDate { get; set; }
    }

    public class GetUserDetailHandler : IRequestHandler<GetUserDetailRequest, GetUserDetailResponse>
    {
        private readonly IUserRepository _userRepo;
        private readonly IUserSession _userSession;

        public GetUserDetailHandler(IUserRepository userRepo, IUserSession userSession)
        {
            _userRepo = userRepo;
            _userSession = userSession;
        }

        public async Task<GetUserDetailResponse> Handle(GetUserDetailRequest request, CancellationToken ct)
        {
            try
            {
                if (request.UserId == Guid.Empty)
                    return new GetUserDetailResponse(false, "UserId không hợp lệ.");

                var user = await _userRepo.GetByIdInStoreAsync(request.UserId, _userSession.StoreId, ct);
                if (user is null)
                    return new GetUserDetailResponse(false, "Không tìm thấy nhân viên trong cửa hàng hiện tại.");

                return new GetUserDetailResponse(new UserDetailDto
                {
                    Id = user.Id,
                    Code = user.Code,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = UserRoleNameResolver.Resolve(user),
                    StoreId = user.StoreId,
                    StoreName = user.Store?.Name ?? string.Empty,
                    IsActive = user.IsActive,
                    CreatedDate = user.CreatedDate,
                    UpdatedDate = user.UpdatedDate,
                    RoleCount = user.UserRoles.Count(ur => !ur.IsDeleted),
                    DirectPermissionCount = user.UserPermissions.Count(up => !up.IsDeleted)
                });
            }
            catch
            {
                return new GetUserDetailResponse(false, "Không thể tải thông tin nhân viên. Vui lòng thử lại.");
            }
        }
    }
}
