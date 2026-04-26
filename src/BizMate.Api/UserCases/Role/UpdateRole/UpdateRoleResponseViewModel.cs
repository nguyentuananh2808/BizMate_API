namespace BizMate.Api.UserCases.Role.UpdateRole
{
    public class UpdateRoleResponseViewModel
    {
        public bool Success { get; set; }
        public string Message { get; set; } = default!;

        public UpdateRoleResponseViewModel(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
    public class UpdateRoleRequestBody
    {
        public string DisplayName { get; set; } = default!;
        public List<Guid> PermissionIds { get; set; } = new();
    }
}
