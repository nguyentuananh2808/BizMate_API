using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Responses;
using BizMate.Domain.Entities;
using MediatR;

namespace BizMate.Application.UserCases.User.Commands.AssignRole
{
    public class AssignRoleRequest : IRequest<AssignRoleResponse>
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public Guid StoreId { get; set; }
    }

    public class AssignRoleResponse : BaseResponse
    {
        public AssignRoleResponse(bool success = true, string? message = null)
            : base(success, message) { }
    }

    public class AssignRoleHandler : IRequestHandler<AssignRoleRequest, AssignRoleResponse>
    {
        private readonly IRoleRepository _roleRepo;
        private readonly IUserRoleRepository _userRoleRepo;
        private readonly IUserRepository _userRepo;
        private readonly ITechnicianHoldingRepository _technicianRepo;
        private readonly ICodeGeneratorService _codeGeneratorService;
        private readonly IUnitOfWork _uow;

        public AssignRoleHandler(
            IRoleRepository roleRepo,
            IUserRoleRepository userRoleRepo,
            IUserRepository userRepo,
            ITechnicianHoldingRepository technicianRepo,
            ICodeGeneratorService codeGeneratorService,
            IUnitOfWork uow)
        {
            _roleRepo = roleRepo;
            _userRoleRepo = userRoleRepo;
            _userRepo = userRepo;
            _technicianRepo = technicianRepo;
            _codeGeneratorService = codeGeneratorService;
            _uow = uow;
        }

        /// <summary>
        /// Assigns a role to a store user and creates the technician profile when needed.
        /// </summary>
        public async Task<AssignRoleResponse> Handle(AssignRoleRequest request, CancellationToken ct)
        {
            try
            {
                var role = await _roleRepo.GetByIdAsync(request.RoleId, ct);
                if (role is null)
                    return new AssignRoleResponse(false, "Không tìm thấy vai trò.");

                if (role.IsSystem &&
                    string.Equals(role.Name, "Owner", StringComparison.OrdinalIgnoreCase))
                {
                    return new AssignRoleResponse(
                        false,
                        "Không thể gán vai trò chủ công ty cho nhân viên.");
                }

                var user = await _userRepo.GetByIdInStoreAsync(
                    request.UserId,
                    request.StoreId,
                    ct);
                if (user is null)
                    return new AssignRoleResponse(false, "Không tìm thấy nhân viên trong công ty hiện tại.");

                if (string.Equals(user.Role, "Owner", StringComparison.OrdinalIgnoreCase))
                    return new AssignRoleResponse(false, "Không thể thay đổi vai trò tài khoản chủ công ty.");

                var alreadyAssigned = await _userRoleRepo.ExistsAsync(
                    request.UserId,
                    request.RoleId,
                    request.StoreId,
                    ct);
                if (alreadyAssigned)
                    return new AssignRoleResponse(false, $"Nhân viên đã có vai trò '{role.DisplayName}'.");

                await _userRoleRepo.AddAsync(new UserRole
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    RoleId = request.RoleId,
                    StoreId = request.StoreId,
                    CreatedDate = DateTime.UtcNow
                }, ct);

                user.Role = role.Name;
                await _userRepo.UpdateAsync(user, ct);

                if (string.Equals(role.Name, "Technician", StringComparison.OrdinalIgnoreCase))
                {
                    await EnsureTechnicianProfileAsync(user, request.StoreId, ct);
                }

                await _uow.SaveChangesAsync(ct);

                return new AssignRoleResponse(true, $"Đã gán vai trò '{role.DisplayName}' thành công.");
            }
            catch
            {
                return new AssignRoleResponse(false, "Không thể gán vai trò cho nhân viên. Vui lòng thử lại.");
            }
        }

        /// <summary>
        /// Creates or reactivates the technician profile linked to a technician user.
        /// </summary>
        private async Task EnsureTechnicianProfileAsync(
            BizMate.Domain.Entities.User user,
            Guid storeId,
            CancellationToken ct)
        {
            var technician = await _technicianRepo.GetTechnicianByUserIdAsync(
                user.Id,
                storeId,
                ct);

            if (technician is null)
            {
                _technicianRepo.AddTechnician(new Technician
                {
                    Id = Guid.NewGuid(),
                    Code = await _codeGeneratorService.GenerateCodeAsync("#KT", 5),
                    UserId = user.Id,
                    Name = user.FullName,
                    StoreId = storeId,
                    IsActive = user.IsActive,
                    CreatedDate = DateTime.UtcNow
                });
                return;
            }

            technician.Name = user.FullName;
            technician.IsActive = user.IsActive;
            technician.UpdatedDate = DateTime.UtcNow;
            technician.RowVersion = Guid.NewGuid();
        }
    }
}

namespace BizMate.Application.UserCases.User.Commands.RevokeRole
{
    public class RevokeRoleRequest : IRequest<RevokeRoleResponse>
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public Guid StoreId { get; set; }

        public RevokeRoleRequest(Guid userId, Guid roleId, Guid storeId)
        {
            UserId = userId;
            RoleId = roleId;
            StoreId = storeId;
        }
    }

    public class RevokeRoleResponse : BaseResponse
    {
        public RevokeRoleResponse(bool success = true, string? message = null)
            : base(success, message) { }
    }

    public class RevokeRoleHandler : IRequestHandler<RevokeRoleRequest, RevokeRoleResponse>
    {
        private readonly IUserRoleRepository _userRoleRepo;
        private readonly IUserRepository _userRepo;
        private readonly IUnitOfWork _uow;

        public RevokeRoleHandler(
            IUserRoleRepository userRoleRepo,
            IUserRepository userRepo,
            IUnitOfWork uow)
        {
            _userRoleRepo = userRoleRepo;
            _userRepo = userRepo;
            _uow = uow;
        }

        /// <summary>
        /// Revokes a role from a user and keeps the legacy User.Role field in sync.
        /// </summary>
        public async Task<RevokeRoleResponse> Handle(RevokeRoleRequest request, CancellationToken ct)
        {
            try
            {
                var userRoles = await _userRoleRepo.GetByUserIdAndStoreIdAsync(
                    request.UserId,
                    request.StoreId,
                    ct);

                var target = userRoles.FirstOrDefault(ur => ur.RoleId == request.RoleId);
                if (target is null)
                    return new RevokeRoleResponse(false, "Nhân viên không có vai trò này.");

                if (target.Role?.IsSystem == true)
                    return new RevokeRoleResponse(false, "Không thể thu hồi vai trò hệ thống.");

                var user = await _userRepo.GetByIdInStoreAsync(
                    request.UserId,
                    request.StoreId,
                    ct);
                if (user is null)
                    return new RevokeRoleResponse(false, "Không tìm thấy nhân viên trong công ty hiện tại.");

                await _userRoleRepo.DeleteAsync(target, ct);
                user.Role = userRoles
                    .Where(ur =>
                        ur.RoleId != request.RoleId &&
                        !ur.IsDeleted &&
                        ur.Role is not null &&
                        !ur.Role.IsDeleted)
                    .OrderByDescending(ur => ur.CreatedDate)
                    .Select(ur => ur.Role.Name)
                    .FirstOrDefault() ?? "Staff";
                await _userRepo.UpdateAsync(user, ct);

                await _uow.SaveChangesAsync(ct);

                return new RevokeRoleResponse(true, "Thu hồi vai trò thành công.");
            }
            catch
            {
                return new RevokeRoleResponse(false, "Không thể gỡ vai trò khỏi nhân viên. Vui lòng thử lại.");
            }
        }
    }
}
