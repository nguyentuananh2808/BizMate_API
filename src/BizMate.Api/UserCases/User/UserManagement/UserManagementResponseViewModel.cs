using BizMate.Application.UserCases.User.Queries.UserManagement;

namespace BizMate.Api.UserCases.User.UserManagement
{
    public class GetUsersResponseViewModel
    {
        public IEnumerable<UserListItemDto> Users { get; set; }
        public int TotalCount { get; set; }

        public GetUsersResponseViewModel(IEnumerable<UserListItemDto> users, int totalCount)
        {
            Users = users;
            TotalCount = totalCount;
        }
    }

    public class UserMutationResponseViewModel
    {
        public bool Success { get; set; }
        public string Message { get; set; } = default!;
        public Guid? UserId { get; set; }

        public UserMutationResponseViewModel(bool success, string message, Guid? userId)
        {
            Success = success;
            Message = message;
            UserId = userId;
        }
    }

    public class CreateStoreUserBody
    {
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string? Role { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateStoreUserBody
    {
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? Role { get; set; }
        public bool IsActive { get; set; }
    }
}
