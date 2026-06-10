namespace BizMate.Api.UserCases.User.UserPermissions
{
    public class UserPermissionsBody
    {
        public List<Guid> PermissionIds { get; set; } = new();
    }

    public class UserPermissionMutationResponseViewModel
    {
        public bool Success { get; set; }
        public string Message { get; set; } = default!;
        public List<Guid> PermissionIds { get; set; } = new();

        public UserPermissionMutationResponseViewModel(
            bool success,
            string message,
            List<Guid> permissionIds)
        {
            Success = success;
            Message = message;
            PermissionIds = permissionIds;
        }
    }
}
