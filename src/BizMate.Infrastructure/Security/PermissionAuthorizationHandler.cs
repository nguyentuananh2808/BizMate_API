// FILE: src/BizMate.Infrastructure/Security/PermissionAuthorizationHandler.cs
// BugFix: Thêm đầy đủ using directives cho Microsoft.AspNetCore.Authorization
// Cần package: Microsoft.AspNetCore.Authorization (thêm vào BizMate.Infrastructure.csproj)

using Microsoft.AspNetCore.Authorization;

namespace BizMate.Infrastructure.Security
{
    /// <summary>
    /// Requirement: yêu cầu JWT phải có claim "permission" với giá trị cụ thể.
    /// </summary>
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }
        public PermissionRequirement(string permission) => Permission = permission;
    }

    /// <summary>
    /// Handler: chỉ đọc claims từ JWT, KHÔNG query DB.
    /// Permission đã được nhúng vào JWT lúc login bởi JwtFactory.
    /// </summary>
    public class PermissionAuthorizationHandler
        : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var hasPerm = context.User.Claims
                .Any(c => c.Type == "permission" && c.Value == requirement.Permission);

            if (hasPerm)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
