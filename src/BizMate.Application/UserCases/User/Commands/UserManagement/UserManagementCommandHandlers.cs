using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Responses;
using BizMate.Application.Common.Security;
using BizMate.Domain.Entities;
using MediatR;
using System.Net.Mail;
using System.Text.RegularExpressions;

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
        public string? Phone { get; set; }
        public Guid RoleId { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateStoreUserRequest : IRequest<UserMutationResponse>
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? Phone { get; set; }
        public Guid? RoleId { get; set; }
        public bool IsActive { get; set; }
    }

    public class DeleteStoreUserRequest : IRequest<UserMutationResponse>
    {
        public Guid UserId { get; set; }
    }

    internal static partial class UserAccountRules
    {
        public static string? ValidateIdentity(string? fullName, string? email)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return "Tên nhân viên không được để trống.";

            if (string.IsNullOrWhiteSpace(email))
                return "Email không được để trống.";

            try
            {
                _ = new MailAddress(email.Trim());
            }
            catch (FormatException)
            {
                return "Email không đúng định dạng.";
            }

            return null;
        }

        public static string? ValidatePassword(string? password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                return "Mật khẩu phải có ít nhất 8 ký tự.";

            if (!UppercaseRegex().IsMatch(password) ||
                !LowercaseRegex().IsMatch(password) ||
                !DigitRegex().IsMatch(password) ||
                !SpecialCharacterRegex().IsMatch(password))
            {
                return "Mật khẩu phải có chữ hoa, chữ thường, số và ký tự đặc biệt.";
            }

            return null;
        }

        public static bool IsOwnerRole(Role role) =>
            role.IsSystem &&
            string.Equals(role.Name, "Owner", StringComparison.OrdinalIgnoreCase);

        public static bool IsTechnicianRole(Role role) =>
            string.Equals(role.Name, "Technician", StringComparison.OrdinalIgnoreCase);

        [GeneratedRegex("[A-Z]")]
        private static partial Regex UppercaseRegex();

        [GeneratedRegex("[a-z]")]
        private static partial Regex LowercaseRegex();

        [GeneratedRegex(@"\d")]
        private static partial Regex DigitRegex();

        [GeneratedRegex(@"[\W_]")]
        private static partial Regex SpecialCharacterRegex();
    }

    public sealed class CreateStoreUserHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        ITechnicianHoldingRepository technicianRepository,
        IUserSession userSession,
        ICodeGeneratorService codeGeneratorService,
        IUnitOfWork unitOfWork)
        : IRequestHandler<CreateStoreUserRequest, UserMutationResponse>
    {
        public async Task<UserMutationResponse> Handle(
            CreateStoreUserRequest request,
            CancellationToken cancellationToken)
        {
            var fullName = request.FullName?.Trim();
            var email = request.Email?.Trim().ToLowerInvariant();
            var identityError = UserAccountRules.ValidateIdentity(fullName, email);
            if (identityError is not null)
                return new UserMutationResponse(false, identityError);

            var passwordError = UserAccountRules.ValidatePassword(request.Password);
            if (passwordError is not null)
                return new UserMutationResponse(false, passwordError);

            if (request.RoleId == Guid.Empty)
                return new UserMutationResponse(false, "Vui lòng chọn vai trò cho nhân viên.");

            if (await userRepository.ExistsByEmailAsync(email!, null, cancellationToken))
                return new UserMutationResponse(false, "Email đã được sử dụng.");

            var role = await roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
            if (role is null)
                return new UserMutationResponse(false, "Vai trò được chọn không tồn tại.");

            if (UserAccountRules.IsOwnerRole(role))
                return new UserMutationResponse(false, "Không thể tạo thêm tài khoản chủ công ty.");

            var creatorId = Guid.TryParse(userSession.UserId, out var parsedCreatorId)
                ? parsedCreatorId
                : (Guid?)null;
            var (passwordHash, passwordSalt) = PasswordHasher.HashWithSalt(request.Password);
            var user = new BizMate.Domain.Entities.User
            {
                Id = Guid.NewGuid(),
                Code = await codeGeneratorService.GenerateCodeAsync("#U", 5),
                FullName = fullName!,
                Email = email!,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = role.Name,
                StoreId = userSession.StoreId,
                IsActive = request.IsActive,
                CreatedBy = creatorId,
                CreatedDate = DateTime.UtcNow
            };

            await unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                await userRepository.AddAsync(user, cancellationToken);
                await userRoleRepository.AddAsync(new UserRole
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    RoleId = role.Id,
                    StoreId = userSession.StoreId,
                    CreatedBy = creatorId,
                    CreatedDate = DateTime.UtcNow
                }, cancellationToken);

                if (UserAccountRules.IsTechnicianRole(role))
                {
                    technicianRepository.AddTechnician(new Technician
                    {
                        Id = Guid.NewGuid(),
                        Code = await codeGeneratorService.GenerateCodeAsync("#KT", 5),
                        UserId = user.Id,
                        Name = user.FullName,
                        Phone = request.Phone?.Trim(),
                        ZaloPhone = request.Phone?.Trim(),
                        StoreId = userSession.StoreId,
                        IsActive = user.IsActive,
                        CreatedBy = creatorId,
                        CreatedDate = DateTime.UtcNow
                    });
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitAsync(cancellationToken);
                return new UserMutationResponse(user.Id, "Tạo tài khoản nhân viên thành công.");
            }
            catch
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new UserMutationResponse(
                    false,
                    "Không thể tạo tài khoản nhân viên. Vui lòng thử lại.");
            }
        }
    }

    public sealed class UpdateStoreUserHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        ITechnicianHoldingRepository technicianRepository,
        IUserSession userSession,
        ICodeGeneratorService codeGeneratorService,
        IUnitOfWork unitOfWork)
        : IRequestHandler<UpdateStoreUserRequest, UserMutationResponse>
    {
        public async Task<UserMutationResponse> Handle(
            UpdateStoreUserRequest request,
            CancellationToken cancellationToken)
        {
            if (request.UserId == Guid.Empty)
                return new UserMutationResponse(false, "UserId không hợp lệ.");

            var fullName = request.FullName?.Trim();
            var email = request.Email?.Trim().ToLowerInvariant();
            var identityError = UserAccountRules.ValidateIdentity(fullName, email);
            if (identityError is not null)
                return new UserMutationResponse(false, identityError);

            var user = await userRepository.GetByIdInStoreAsync(
                request.UserId,
                userSession.StoreId,
                cancellationToken);
            if (user is null)
                return new UserMutationResponse(false, "Không tìm thấy nhân viên trong công ty hiện tại.");

            if (await userRepository.ExistsByEmailAsync(email!, request.UserId, cancellationToken))
                return new UserMutationResponse(false, "Email đã được sử dụng.");

            Role? selectedRole = null;
            if (request.RoleId.HasValue)
            {
                if (request.RoleId.Value == Guid.Empty)
                    return new UserMutationResponse(false, "Vai trò không hợp lệ.");

                selectedRole = await roleRepository.GetByIdAsync(
                    request.RoleId.Value,
                    cancellationToken);
                if (selectedRole is null)
                    return new UserMutationResponse(false, "Vai trò được chọn không tồn tại.");

                if (UserAccountRules.IsOwnerRole(selectedRole))
                    return new UserMutationResponse(false, "Không thể gán vai trò chủ công ty cho nhân viên.");

                if (string.Equals(user.Role, "Owner", StringComparison.OrdinalIgnoreCase))
                    return new UserMutationResponse(false, "Không thể thay đổi vai trò tài khoản chủ công ty.");
            }

            if (string.Equals(user.Role, "Owner", StringComparison.OrdinalIgnoreCase) &&
                !request.IsActive)
            {
                return new UserMutationResponse(false, "Không thể khóa tài khoản chủ công ty.");
            }

            await unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                user.FullName = fullName!;
                user.Email = email!;
                user.IsActive = request.IsActive;
                user.UpdatedDate = DateTime.UtcNow;

                if (selectedRole is not null)
                {
                    var currentRoles = await userRoleRepository.GetByUserIdAndStoreIdAsync(
                        user.Id,
                        userSession.StoreId,
                        cancellationToken);
                    if (currentRoles.Count > 0)
                        await userRoleRepository.DeleteRangeAsync(currentRoles, cancellationToken);

                    await userRoleRepository.AddAsync(new UserRole
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        RoleId = selectedRole.Id,
                        StoreId = userSession.StoreId,
                        UpdatedBy = Guid.TryParse(userSession.UserId, out var editorId)
                            ? editorId
                            : null,
                        CreatedDate = DateTime.UtcNow
                    }, cancellationToken);
                    user.Role = selectedRole.Name;
                }

                var linkedTechnician = await technicianRepository.GetTechnicianByUserIdAsync(
                    user.Id,
                    userSession.StoreId,
                    cancellationToken);
                var shouldBeTechnician = selectedRole is not null
                    ? UserAccountRules.IsTechnicianRole(selectedRole)
                    : string.Equals(user.Role, "Technician", StringComparison.OrdinalIgnoreCase);

                if (shouldBeTechnician)
                {
                    if (linkedTechnician is null)
                    {
                        technicianRepository.AddTechnician(new Technician
                        {
                            Id = Guid.NewGuid(),
                            Code = await codeGeneratorService.GenerateCodeAsync("#KT", 5),
                            UserId = user.Id,
                            Name = user.FullName,
                            Phone = request.Phone?.Trim(),
                            ZaloPhone = request.Phone?.Trim(),
                            StoreId = userSession.StoreId,
                            IsActive = user.IsActive,
                            CreatedBy = Guid.TryParse(userSession.UserId, out var creatorId)
                                ? creatorId
                                : null,
                            CreatedDate = DateTime.UtcNow
                        });
                    }
                    else
                    {
                        linkedTechnician.Name = user.FullName;
                        linkedTechnician.Phone = request.Phone?.Trim() ?? linkedTechnician.Phone;
                        linkedTechnician.ZaloPhone = request.Phone?.Trim() ?? linkedTechnician.ZaloPhone;
                        linkedTechnician.IsActive = user.IsActive;
                        linkedTechnician.UpdatedDate = DateTime.UtcNow;
                    }
                }
                else if (linkedTechnician is not null)
                {
                    linkedTechnician.IsActive = false;
                    linkedTechnician.UpdatedDate = DateTime.UtcNow;
                }

                await userRepository.UpdateAsync(user, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitAsync(cancellationToken);
                return new UserMutationResponse(user.Id, "Cập nhật nhân viên thành công.");
            }
            catch
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new UserMutationResponse(
                    false,
                    "Không thể cập nhật nhân viên. Vui lòng thử lại.");
            }
        }
    }

    public sealed class DeleteStoreUserHandler(
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository,
        IUserPermissionRepository userPermissionRepository,
        ITechnicianHoldingRepository technicianRepository,
        IUserSession userSession,
        IUnitOfWork unitOfWork)
        : IRequestHandler<DeleteStoreUserRequest, UserMutationResponse>
    {
        public async Task<UserMutationResponse> Handle(
            DeleteStoreUserRequest request,
            CancellationToken cancellationToken)
        {
            if (request.UserId == Guid.Empty)
                return new UserMutationResponse(false, "UserId không hợp lệ.");

            if (Guid.TryParse(userSession.UserId, out var currentUserId) &&
                currentUserId == request.UserId)
            {
                return new UserMutationResponse(false, "Không thể xóa tài khoản đang đăng nhập.");
            }

            var user = await userRepository.GetByIdInStoreAsync(
                request.UserId,
                userSession.StoreId,
                cancellationToken);
            if (user is null)
                return new UserMutationResponse(false, "Không tìm thấy nhân viên trong công ty hiện tại.");

            if (string.Equals(user.Role, "Owner", StringComparison.OrdinalIgnoreCase))
                return new UserMutationResponse(false, "Không thể xóa tài khoản chủ công ty.");

            await unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var userRoles = await userRoleRepository.GetByUserIdAndStoreIdAsync(
                    user.Id,
                    userSession.StoreId,
                    cancellationToken);
                if (userRoles.Count > 0)
                    await userRoleRepository.DeleteRangeAsync(userRoles, cancellationToken);

                var userPermissions = await userPermissionRepository.GetByUserIdAndStoreIdAsync(
                    user.Id,
                    userSession.StoreId,
                    cancellationToken);
                if (userPermissions.Count > 0)
                    await userPermissionRepository.DeleteRangeAsync(userPermissions, cancellationToken);

                var linkedTechnician = await technicianRepository.GetTechnicianByUserIdAsync(
                    user.Id,
                    userSession.StoreId,
                    cancellationToken);
                if (linkedTechnician is not null)
                {
                    linkedTechnician.IsActive = false;
                    linkedTechnician.IsDeleted = true;
                    linkedTechnician.UpdatedDate = DateTime.UtcNow;
                }

                await userRepository.DeleteAsync(user, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitAsync(cancellationToken);
                return new UserMutationResponse(user.Id, "Xóa nhân viên thành công.");
            }
            catch
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new UserMutationResponse(false, "Không thể xóa nhân viên. Vui lòng thử lại.");
            }
        }
    }
}
