using BizMate.Domain.Entities;

namespace BizMate.Application.Common.Users
{
    internal static class UserRoleNameResolver
    {
        /// <summary>
        /// Gets the active role name from role assignments and falls back to the legacy User.Role field.
        /// </summary>
        public static string Resolve(User user)
        {
            var assignedRole = user.UserRoles
                .Where(userRole =>
                    !userRole.IsDeleted &&
                    userRole.Role is not null &&
                    !userRole.Role.IsDeleted)
                .OrderByDescending(userRole => userRole.CreatedDate)
                .Select(userRole => userRole.Role.Name)
                .FirstOrDefault();

            return string.IsNullOrWhiteSpace(assignedRole)
                ? user.Role
                : assignedRole;
        }
    }
}
