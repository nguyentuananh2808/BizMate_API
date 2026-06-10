using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Responses;
using BizMate.Application.Common.Security;
using MediatR;

namespace BizMate.Application.UserCases.User.Commands.UserManagement
{
    public class UserMutationResponse : BaseResponse
    {
        public Guid? UserId { get; set; }

        public UserMutationResponse(Guid userId, string message) : base(true, message)
            => UserId = userId;

        public UserMutationResponse(bool success = false, string? message = null)
            : base(success, message) { }
    }

    public class CreateStoreUserRequest : IRequest<UserMutationResponse>
    {
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string? Role { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateStoreUserRequest : IRequest<UserMutationResponse>
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? Role { get; set; }
        public bool IsActive { get; set; }
    }

    public class DeleteStoreUserRequest : IRequest<UserMutationResponse>
    {
        public Guid UserId { get; set; }
    }

    public class CreateStoreUserHandler : IRequestHandler<CreateStoreUserRequest, UserMutationResponse>
    {
        private readonly IUserRepository _userRepo;
        private readonly IUserSession _userSession;
        private readonly ICodeGeneratorService _codeGeneratorService;

        public CreateStoreUserHandler(
            IUserRepository userRepo,
            IUserSession userSession,
            ICodeGeneratorService codeGeneratorService)
        {
            _userRepo = userRepo;
            _userSession = userSession;
            _codeGeneratorService = codeGeneratorService;
        }

        public async Task<UserMutationResponse> Handle(CreateStoreUserRequest request, CancellationToken ct)
        {
            try
            {
                var fullName = request.FullName?.Trim();
                var email = request.Email?.Trim().ToLower();
                var role = string.IsNullOrWhiteSpace(request.Role) ? "Staff" : request.Role.Trim();

                if (string.IsNullOrWhiteSpace(fullName))
                    return new UserMutationResponse(false, "Tên nhân viên không được để trống.");
                if (string.IsNullOrWhiteSpace(email))
                    return new UserMutationResponse(false, "Email không được để trống.");
                if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
                    return new UserMutationResponse(false, "Mật khẩu phải có ít nhất 6 ký tự.");
                if (await _userRepo.ExistsByEmailAsync(email, null, ct))
                    return new UserMutationResponse(false, "Email đã được sử dụng.");

                var (passwordHash, passwordSalt) = PasswordHasher.HashWithSalt(request.Password);
                var user = new BizMate.Domain.Entities.User
                {
                    Id = Guid.NewGuid(),
                    Code = await _codeGeneratorService.GenerateCodeAsync("#U", 5),
                    FullName = fullName,
                    Email = email,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    Role = role,
                    StoreId = _userSession.StoreId,
                    IsActive = request.IsActive,
                    CreatedDate = DateTime.UtcNow
                };

                await _userRepo.AddAsync(user, ct);
                return new UserMutationResponse(user.Id, "Tạo nhân viên thành công.");
            }
            catch (Exception ex)
            {
                return new UserMutationResponse(false, ex.Message);
            }
        }
    }

    public class UpdateStoreUserHandler : IRequestHandler<UpdateStoreUserRequest, UserMutationResponse>
    {
        private readonly IUserRepository _userRepo;
        private readonly IUserSession _userSession;
        private readonly IUnitOfWork _uow;

        public UpdateStoreUserHandler(
            IUserRepository userRepo,
            IUserSession userSession,
            IUnitOfWork uow)
        {
            _userRepo = userRepo;
            _userSession = userSession;
            _uow = uow;
        }

        public async Task<UserMutationResponse> Handle(UpdateStoreUserRequest request, CancellationToken ct)
        {
            try
            {
                var fullName = request.FullName?.Trim();
                var email = request.Email?.Trim().ToLower();
                var role = string.IsNullOrWhiteSpace(request.Role) ? "Staff" : request.Role.Trim();

                if (request.UserId == Guid.Empty)
                    return new UserMutationResponse(false, "UserId không hợp lệ.");
                if (string.IsNullOrWhiteSpace(fullName))
                    return new UserMutationResponse(false, "Tên nhân viên không được để trống.");
                if (string.IsNullOrWhiteSpace(email))
                    return new UserMutationResponse(false, "Email không được để trống.");

                var user = await _userRepo.GetByIdInStoreAsync(request.UserId, _userSession.StoreId, ct);
                if (user is null)
                    return new UserMutationResponse(false, "Không tìm thấy nhân viên trong cửa hàng hiện tại.");
                if (await _userRepo.ExistsByEmailAsync(email, request.UserId, ct))
                    return new UserMutationResponse(false, "Email đã được sử dụng.");

                user.FullName = fullName;
                user.Email = email;
                user.Role = role;
                user.IsActive = request.IsActive;
                user.UpdatedDate = DateTime.UtcNow;

                await _userRepo.UpdateAsync(user, ct);
                await _uow.SaveChangesAsync(ct);

                return new UserMutationResponse(user.Id, "Cập nhật nhân viên thành công.");
            }
            catch (Exception ex)
            {
                return new UserMutationResponse(false, ex.Message);
            }
        }
    }

    public class DeleteStoreUserHandler : IRequestHandler<DeleteStoreUserRequest, UserMutationResponse>
    {
        private readonly IUserRepository _userRepo;
        private readonly IUserRoleRepository _userRoleRepo;
        private readonly IUserPermissionRepository _userPermissionRepo;
        private readonly IUserSession _userSession;
        private readonly IUnitOfWork _uow;

        public DeleteStoreUserHandler(
            IUserRepository userRepo,
            IUserRoleRepository userRoleRepo,
            IUserPermissionRepository userPermissionRepo,
            IUserSession userSession,
            IUnitOfWork uow)
        {
            _userRepo = userRepo;
            _userRoleRepo = userRoleRepo;
            _userPermissionRepo = userPermissionRepo;
            _userSession = userSession;
            _uow = uow;
        }

        public async Task<UserMutationResponse> Handle(DeleteStoreUserRequest request, CancellationToken ct)
        {
            try
            {
                if (request.UserId == Guid.Empty)
                    return new UserMutationResponse(false, "UserId không hợp lệ.");

                if (Guid.TryParse(_userSession.UserId, out var currentUserId) && currentUserId == request.UserId)
                    return new UserMutationResponse(false, "Không thể xóa chính tài khoản đang đăng nhập.");

                var user = await _userRepo.GetByIdInStoreAsync(request.UserId, _userSession.StoreId, ct);
                if (user is null)
                    return new UserMutationResponse(false, "Không tìm thấy nhân viên trong cửa hàng hiện tại.");
                if (string.Equals(user.Role, "Owner", StringComparison.OrdinalIgnoreCase))
                    return new UserMutationResponse(false, "Không thể xóa tài khoản chủ cửa hàng.");

                var userRoles = await _userRoleRepo.GetByUserIdAndStoreIdAsync(user.Id, _userSession.StoreId, ct);
                if (userRoles.Count > 0)
                    await _userRoleRepo.DeleteRangeAsync(userRoles, ct);

                var userPermissions = await _userPermissionRepo.GetByUserIdAndStoreIdAsync(user.Id, _userSession.StoreId, ct);
                if (userPermissions.Count > 0)
                    await _userPermissionRepo.DeleteRangeAsync(userPermissions, ct);

                await _userRepo.DeleteAsync(user, ct);
                await _uow.SaveChangesAsync(ct);

                return new UserMutationResponse(user.Id, "Xóa nhân viên thành công.");
            }
            catch (Exception ex)
            {
                return new UserMutationResponse(false, ex.Message);
            }
        }
    }
}
