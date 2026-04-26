namespace BizMate.Api.UserCases.Role.DeleteRole
{
    public class DeleteRolesResponseViewModel
    {
        public bool Success { get; set; }
        public string Message { get; set; } = default!;

        public DeleteRolesResponseViewModel(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
