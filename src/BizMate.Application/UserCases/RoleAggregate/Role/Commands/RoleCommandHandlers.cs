using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Responses;
using BizMate.Domain.Entities;
using FluentValidation;
using MediatR;

// ══════════════════════════════════════════════════════════════════════════════
// CREATE ROLE
// ══════════════════════════════════════════════════════════════════════════════
namespace BizMate.Application.UserCases.RoleAggregate.Role.Commands.CreateRole
{
    public class CreateRoleRequest : IRequest<CreateRoleResponse>
    {
        public string Name { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public List<Guid> PermissionIds { get; set; } = new();
    }

    public class CreateRoleResponse : BaseResponse
    {
        public Guid RoleId { get; set; }
        public CreateRoleResponse(Guid roleId) : base(true) => RoleId = roleId;
        public CreateRoleResponse(bool success = false, string? message = null) : base(success, message) { }
    }

    public class CreateRoleRequestValidator : AbstractValidator<CreateRoleRequest>
    {
        public CreateRoleRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(200);
            RuleFor(x => x.PermissionIds).NotNull();
        }
    }

    public class CreateRoleHandler : IRequestHandler<CreateRoleRequest, CreateRoleResponse>
    {
        private readonly IRoleRepository _roleRepo;
        private readonly IPermissionRepository _permRepo;
        private readonly IUnitOfWork _uow;

        public CreateRoleHandler(IRoleRepository roleRepo, IPermissionRepository permRepo, IUnitOfWork uow)
        {
            _roleRepo = roleRepo;
            _permRepo = permRepo;
            _uow      = uow;
        }

        public async Task<CreateRoleResponse> Handle(CreateRoleRequest request, CancellationToken ct)
        {
            try
            {
                if (await _roleRepo.ExistsByNameAsync(request.Name, ct))
                    return new CreateRoleResponse(false, $"Vai trò '{request.Name}' đã tồn tại.");

                var permissions = await _permRepo.GetByIdsAsync(request.PermissionIds, ct);
                if (permissions.Count != request.PermissionIds.Count)
                    return new CreateRoleResponse(false, "Một số permission không tồn tại.");

                var role = new Domain.Entities.Role
                {
                    Id          = Guid.NewGuid(),
                    Name        = request.Name,
                    DisplayName = request.DisplayName,
                    IsSystem    = false,
                    CreatedDate = DateTime.UtcNow,
                    RolePermissions = permissions.Select(p => new RolePermission
                    {
                        Id           = Guid.NewGuid(),
                        PermissionId = p.Id,
                        CreatedDate  = DateTime.UtcNow,
                    }).ToList()
                };

                await _roleRepo.AddAsync(role, ct);
                await _uow.SaveChangesAsync(ct);

                return new CreateRoleResponse(role.Id);
            }
            catch (Exception ex)
            {
                return new CreateRoleResponse(false, ex.Message);
            }
        }
    }
}

// ══════════════════════════════════════════════════════════════════════════════
// UPDATE ROLE
// ══════════════════════════════════════════════════════════════════════════════
namespace BizMate.Application.UserCases.RoleAggregate.Role.Commands.UpdateRole
{
    public class UpdateRoleRequest : IRequest<UpdateRoleResponse>
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; } = default!;
        public List<Guid> PermissionIds { get; set; } = new();
    }

    public class UpdateRoleResponse : BaseResponse
    {
        public UpdateRoleResponse(bool success = true, string? message = null) : base(success, message) { }
    }

    public class UpdateRoleRequestValidator : AbstractValidator<UpdateRoleRequest>
    {
        public UpdateRoleRequestValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(200);
            RuleFor(x => x.PermissionIds).NotNull();
        }
    }

    public class UpdateRoleHandler : IRequestHandler<UpdateRoleRequest, UpdateRoleResponse>
    {
        private readonly IRoleRepository _roleRepo;
        private readonly IPermissionRepository _permRepo;
        private readonly IUnitOfWork _uow;

        public UpdateRoleHandler(IRoleRepository roleRepo, IPermissionRepository permRepo, IUnitOfWork uow)
        {
            _roleRepo = roleRepo;
            _permRepo = permRepo;
            _uow      = uow;
        }

        public async Task<UpdateRoleResponse> Handle(UpdateRoleRequest request, CancellationToken ct)
        {
            try
            {
                var role = await _roleRepo.GetByIdWithPermissionsAsync(request.Id, ct);
                if (role is null)
                    return new UpdateRoleResponse(false, "Không tìm thấy vai trò.");
                if (role.IsSystem)
                    return new UpdateRoleResponse(false, "Không thể chỉnh sửa vai trò hệ thống.");

                var permissions = await _permRepo.GetByIdsAsync(request.PermissionIds, ct);
                if (permissions.Count != request.PermissionIds.Count)
                    return new UpdateRoleResponse(false, "Một số permission không tồn tại.");

                // Ghi đè toàn bộ danh sách permission
                role.DisplayName = request.DisplayName;
                role.UpdatedDate = DateTime.UtcNow;
                role.RolePermissions.Clear();
                foreach (var p in permissions)
                {
                    role.RolePermissions.Add(new RolePermission
                    {
                        Id           = Guid.NewGuid(),
                        RoleId       = role.Id,
                        PermissionId = p.Id,
                        CreatedDate  = DateTime.UtcNow,
                    });
                }

                await _roleRepo.UpdateAsync(role, ct);
                await _uow.SaveChangesAsync(ct);

                return new UpdateRoleResponse(true, "Cập nhật vai trò thành công.");
            }
            catch (Exception ex)
            {
                return new UpdateRoleResponse(false, ex.Message);
            }
        }
    }
}

// ══════════════════════════════════════════════════════════════════════════════
// DELETE ROLE
// ══════════════════════════════════════════════════════════════════════════════
namespace BizMate.Application.UserCases.RoleAggregate.Role.Commands.DeleteRole
{
    public class DeleteRoleRequest : IRequest<DeleteRoleResponse>
    {
        public Guid Id { get; set; }
        public DeleteRoleRequest(Guid id) => Id = id;
    }

    public class DeleteRoleResponse : BaseResponse
    {
        public DeleteRoleResponse(bool success = true, string? message = null) : base(success, message) { }
    }

    public class DeleteRoleHandler : IRequestHandler<DeleteRoleRequest, DeleteRoleResponse>
    {
        private readonly IRoleRepository _roleRepo;
        private readonly IUserRoleRepository _userRoleRepo;
        private readonly IUnitOfWork _uow;

        public DeleteRoleHandler(IRoleRepository roleRepo, IUserRoleRepository userRoleRepo, IUnitOfWork uow)
        {
            _roleRepo     = roleRepo;
            _userRoleRepo = userRoleRepo;
            _uow          = uow;
        }

        public async Task<DeleteRoleResponse> Handle(DeleteRoleRequest request, CancellationToken ct)
        {
            try
            {
                var role = await _roleRepo.GetByIdAsync(request.Id, ct);
                if (role is null)
                    return new DeleteRoleResponse(false, "Không tìm thấy vai trò.");
                if (role.IsSystem)
                    return new DeleteRoleResponse(false, "Không thể xóa vai trò hệ thống.");

                var usersWithRole = await _userRoleRepo.GetByRoleIdAsync(request.Id, ct);
                if (usersWithRole.Any())
                    return new DeleteRoleResponse(false,
                        $"Không thể xóa vai trò đang được gán cho {usersWithRole.Count} nhân viên.");

                await _roleRepo.DeleteAsync(role, ct);
                await _uow.SaveChangesAsync(ct);

                return new DeleteRoleResponse(true, "Xóa vai trò thành công.");
            }
            catch (Exception ex)
            {
                return new DeleteRoleResponse(false, ex.Message);
            }
        }
    }
}
