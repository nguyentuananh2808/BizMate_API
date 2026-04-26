// FILE: src/BizMate.Infrastructure/Security/HasPermissionAttribute.cs
// BugFix: Thêm đầy đủ using directives cho Microsoft.AspNetCore.Authorization
// Cần package: Microsoft.AspNetCore.Authorization (thêm vào BizMate.Infrastructure.csproj)

using Microsoft.AspNetCore.Authorization;

namespace BizMate.Infrastructure.Security
{
    /// <summary>
    /// Attribute bảo vệ endpoint bằng permission string.
    /// Dùng thay thế [Authorize] trên action cụ thể.
    ///
    /// Ví dụ:
    ///   [HasPermission(PermissionConstants.Order.Create)]
    ///   public async Task&lt;IActionResult&gt; Create(...) { }
    /// </summary>
    public sealed class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(string permission)
            : base(policy: permission)
        {
            // Policy name = permission string
            // Policy được đăng ký động trong InfrastructureDependencyInjection
        }
    }
}
