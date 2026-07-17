using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Responses;
using BizMate.Domain.Entities;
using MediatR;

namespace BizMate.Application.UserCases.User.Commands.UserPermissions
{
    public class UserPermissionMutationResponse : BaseResponse
    {
        public List<Guid> PermissionIds { get; set; } = new();

        public UserPermissionMutationResponse(List<Guid> permissionIds, string message)
            : base(true, message)
        {
            PermissionIds = permissionIds;
        }

        public UserPermissionMutationResponse(bool success = false, string? message = null)
            : base(success, message) { }
    }

    public class AddUserPermissionsRequest : IRequest<UserPermissionMutationResponse>
    {
        public Guid UserId { get; set; }
        public Guid StoreId { get; set; }
        public List<Guid> PermissionIds { get; set; } = new();
    }

    public class ReplaceUserPermissionsRequest : IRequest<UserPermissionMutationResponse>
    {
        public Guid UserId { get; set; }
        public Guid StoreId { get; set; }
        public List<Guid> PermissionIds { get; set; } = new();
    }

    public class RemoveUserPermissionRequest : IRequest<UserPermissionMutationResponse>
    {
        public Guid UserId { get; set; }
        public Guid StoreId { get; set; }
        public Guid PermissionId { get; set; }
    }

    public class ClearUserPermissionsRequest : IRequest<UserPermissionMutationResponse>
    {
        public Guid UserId { get; set; }
        public Guid StoreId { get; set; }
    }

    internal static class UserPermissionCommandGuard
    {
        /// <summary>
        /// Removes empty and duplicate permission ids before saving direct permissions.
        /// </summary>
        public static List<Guid> NormalizePermissionIds(IEnumerable<Guid>? permissionIds)
            => (permissionIds ?? Enumerable.Empty<Guid>())
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();

        /// <summary>
        /// Detects payloads that include an empty permission id.
        /// </summary>
        public static bool HasInvalidPermissionId(IEnumerable<Guid>? permissionIds)
            => permissionIds?.Any(id => id == Guid.Empty) == true;
    }

    public class AddUserPermissionsHandler : IRequestHandler<AddUserPermissionsRequest, UserPermissionMutationResponse>
    {
        private readonly IUserRepository _userRepo;
        private readonly IPermissionRepository _permissionRepo;
        private readonly IUserPermissionRepository _userPermissionRepo;
        private readonly IUnitOfWork _uow;

        public AddUserPermissionsHandler(
            IUserRepository userRepo,
            IPermissionRepository permissionRepo,
            IUserPermissionRepository userPermissionRepo,
            IUnitOfWork uow)
        {
            _userRepo = userRepo;
            _permissionRepo = permissionRepo;
            _userPermissionRepo = userPermissionRepo;
            _uow = uow;
        }

        public async Task<UserPermissionMutationResponse> Handle(AddUserPermissionsRequest request, CancellationToken ct)
        {
            try
            {
                var permissionIds = UserPermissionCommandGuard.NormalizePermissionIds(request.PermissionIds);
                var validationError = await ValidateUserAndPermissions(request.UserId, request.StoreId, permissionIds, request.PermissionIds, ct);
                if (validationError is not null)
                    return validationError;

                var current = await _userPermissionRepo.GetByUserIdAndStoreIdAsync(request.UserId, request.StoreId, ct);
                var currentIds = current.Select(up => up.PermissionId).ToHashSet();
                var idsToAdd = permissionIds.Where(id => !currentIds.Contains(id)).ToList();

                if (idsToAdd.Count > 0)
                {
                    var now = DateTime.UtcNow;
                    var userPermissions = idsToAdd.Select(permissionId => new UserPermission
                    {
                        Id = Guid.NewGuid(),
                        UserId = request.UserId,
                        StoreId = request.StoreId,
                        PermissionId = permissionId,
                        CreatedDate = now
                    });

                    await _userPermissionRepo.AddRangeAsync(userPermissions, ct);
                    await _uow.SaveChangesAsync(ct);
                }

                var resultIds = currentIds.Union(idsToAdd).OrderBy(id => id).ToList();
                return new UserPermissionMutationResponse(resultIds, "Thêm quyền cho nhân viên thành công.");
            }
            catch
            {
                return new UserPermissionMutationResponse(false, "Không thể thêm quyền cho nhân viên. Vui lòng thử lại.");
            }
        }

        private async Task<UserPermissionMutationResponse?> ValidateUserAndPermissions(
            Guid userId,
            Guid storeId,
            List<Guid> normalizedPermissionIds,
            List<Guid>? rawPermissionIds,
            CancellationToken ct)
        {
            if (userId == Guid.Empty)
                return new UserPermissionMutationResponse(false, "UserId không hợp lệ.");
            if (UserPermissionCommandGuard.HasInvalidPermissionId(rawPermissionIds))
                return new UserPermissionMutationResponse(false, "PermissionId không hợp lệ.");
            if (normalizedPermissionIds.Count == 0)
                return new UserPermissionMutationResponse(false, "Vui lòng chọn ít nhất một quyền.");
            if (!await _userRepo.ExistsInStoreAsync(userId, storeId, ct))
                return new UserPermissionMutationResponse(false, "Không tìm thấy nhân viên trong cửa hàng hiện tại.");

            var permissions = await _permissionRepo.GetByIdsAsync(normalizedPermissionIds, ct);
            if (permissions.Count != normalizedPermissionIds.Count)
                return new UserPermissionMutationResponse(false, "Một số quyền không tồn tại.");

            return null;
        }
    }

    public class ReplaceUserPermissionsHandler : IRequestHandler<ReplaceUserPermissionsRequest, UserPermissionMutationResponse>
    {
        private readonly IUserRepository _userRepo;
        private readonly IPermissionRepository _permissionRepo;
        private readonly IUserPermissionRepository _userPermissionRepo;
        private readonly IUnitOfWork _uow;

        public ReplaceUserPermissionsHandler(
            IUserRepository userRepo,
            IPermissionRepository permissionRepo,
            IUserPermissionRepository userPermissionRepo,
            IUnitOfWork uow)
        {
            _userRepo = userRepo;
            _permissionRepo = permissionRepo;
            _userPermissionRepo = userPermissionRepo;
            _uow = uow;
        }

        public async Task<UserPermissionMutationResponse> Handle(ReplaceUserPermissionsRequest request, CancellationToken ct)
        {
            try
            {
                var permissionIds = UserPermissionCommandGuard.NormalizePermissionIds(request.PermissionIds);
                var validationError = await ValidateUserAndPermissions(request.UserId, request.StoreId, permissionIds, request.PermissionIds, ct);
                if (validationError is not null)
                    return validationError;

                var current = await _userPermissionRepo.GetByUserIdAndStoreIdAsync(request.UserId, request.StoreId, ct);
                var currentIds = current.Select(up => up.PermissionId).ToHashSet();
                var desiredIds = permissionIds.ToHashSet();

                var toRemove = current.Where(up => !desiredIds.Contains(up.PermissionId)).ToList();
                var idsToAdd = desiredIds.Where(id => !currentIds.Contains(id)).ToList();

                if (toRemove.Count > 0)
                    await _userPermissionRepo.DeleteRangeAsync(toRemove, ct);

                if (idsToAdd.Count > 0)
                {
                    var now = DateTime.UtcNow;
                    var userPermissions = idsToAdd.Select(permissionId => new UserPermission
                    {
                        Id = Guid.NewGuid(),
                        UserId = request.UserId,
                        StoreId = request.StoreId,
                        PermissionId = permissionId,
                        CreatedDate = now
                    });

                    await _userPermissionRepo.AddRangeAsync(userPermissions, ct);
                }

                if (toRemove.Count > 0 || idsToAdd.Count > 0)
                    await _uow.SaveChangesAsync(ct);

                return new UserPermissionMutationResponse(permissionIds.OrderBy(id => id).ToList(), "Cập nhật quyền nhân viên thành công.");
            }
            catch
            {
                return new UserPermissionMutationResponse(false, "Không thể cập nhật quyền nhân viên. Vui lòng thử lại.");
            }
        }

        private async Task<UserPermissionMutationResponse?> ValidateUserAndPermissions(
            Guid userId,
            Guid storeId,
            List<Guid> normalizedPermissionIds,
            List<Guid>? rawPermissionIds,
            CancellationToken ct)
        {
            if (userId == Guid.Empty)
                return new UserPermissionMutationResponse(false, "UserId không hợp lệ.");
            if (UserPermissionCommandGuard.HasInvalidPermissionId(rawPermissionIds))
                return new UserPermissionMutationResponse(false, "PermissionId không hợp lệ.");
            if (!await _userRepo.ExistsInStoreAsync(userId, storeId, ct))
                return new UserPermissionMutationResponse(false, "Không tìm thấy nhân viên trong cửa hàng hiện tại.");

            if (normalizedPermissionIds.Count == 0)
                return null;

            var permissions = await _permissionRepo.GetByIdsAsync(normalizedPermissionIds, ct);
            if (permissions.Count != normalizedPermissionIds.Count)
                return new UserPermissionMutationResponse(false, "Một số quyền không tồn tại.");

            return null;
        }
    }

    public class RemoveUserPermissionHandler : IRequestHandler<RemoveUserPermissionRequest, UserPermissionMutationResponse>
    {
        private readonly IUserRepository _userRepo;
        private readonly IUserPermissionRepository _userPermissionRepo;
        private readonly IUnitOfWork _uow;

        public RemoveUserPermissionHandler(
            IUserRepository userRepo,
            IUserPermissionRepository userPermissionRepo,
            IUnitOfWork uow)
        {
            _userRepo = userRepo;
            _userPermissionRepo = userPermissionRepo;
            _uow = uow;
        }

        public async Task<UserPermissionMutationResponse> Handle(RemoveUserPermissionRequest request, CancellationToken ct)
        {
            try
            {
                if (request.UserId == Guid.Empty)
                    return new UserPermissionMutationResponse(false, "UserId không hợp lệ.");
                if (request.PermissionId == Guid.Empty)
                    return new UserPermissionMutationResponse(false, "PermissionId không hợp lệ.");
                if (!await _userRepo.ExistsInStoreAsync(request.UserId, request.StoreId, ct))
                    return new UserPermissionMutationResponse(false, "Không tìm thấy nhân viên trong cửa hàng hiện tại.");

                var current = await _userPermissionRepo.GetByUserIdAndStoreIdAsync(request.UserId, request.StoreId, ct);
                var target = current.FirstOrDefault(up => up.PermissionId == request.PermissionId);
                if (target is null)
                    return new UserPermissionMutationResponse(false, "Nhân viên chưa được gán quyền này.");

                await _userPermissionRepo.DeleteAsync(target, ct);
                await _uow.SaveChangesAsync(ct);

                var resultIds = current
                    .Where(up => up.PermissionId != request.PermissionId)
                    .Select(up => up.PermissionId)
                    .OrderBy(id => id)
                    .ToList();
                return new UserPermissionMutationResponse(resultIds, "Xóa quyền khỏi nhân viên thành công.");
            }
            catch
            {
                return new UserPermissionMutationResponse(false, "Không thể gỡ quyền khỏi nhân viên. Vui lòng thử lại.");
            }
        }
    }

    public class ClearUserPermissionsHandler : IRequestHandler<ClearUserPermissionsRequest, UserPermissionMutationResponse>
    {
        private readonly IUserRepository _userRepo;
        private readonly IUserPermissionRepository _userPermissionRepo;
        private readonly IUnitOfWork _uow;

        public ClearUserPermissionsHandler(
            IUserRepository userRepo,
            IUserPermissionRepository userPermissionRepo,
            IUnitOfWork uow)
        {
            _userRepo = userRepo;
            _userPermissionRepo = userPermissionRepo;
            _uow = uow;
        }

        public async Task<UserPermissionMutationResponse> Handle(ClearUserPermissionsRequest request, CancellationToken ct)
        {
            try
            {
                if (request.UserId == Guid.Empty)
                    return new UserPermissionMutationResponse(false, "UserId không hợp lệ.");
                if (!await _userRepo.ExistsInStoreAsync(request.UserId, request.StoreId, ct))
                    return new UserPermissionMutationResponse(false, "Không tìm thấy nhân viên trong cửa hàng hiện tại.");

                var current = await _userPermissionRepo.GetByUserIdAndStoreIdAsync(request.UserId, request.StoreId, ct);
                if (current.Count > 0)
                {
                    await _userPermissionRepo.DeleteRangeAsync(current, ct);
                    await _uow.SaveChangesAsync(ct);
                }

                return new UserPermissionMutationResponse(new List<Guid>(), "Đã xóa toàn bộ quyền trực tiếp của nhân viên.");
            }
            catch
            {
                return new UserPermissionMutationResponse(false, "Không thể xóa quyền riêng của nhân viên. Vui lòng thử lại.");
            }
        }
    }
}
