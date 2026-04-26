using BizMate.Application.UserCases.RoleAggregate.Role.Commands.CreateRole;

namespace BizMate.Api.UserCases.Role.CreateRole
{
    public class CreateRolesResponseViewModel
    {
        public Guid RoleId { get; set; }
        public string Message { get; set; } = "Tạo vai trò thành công.";

        public CreateRolesResponseViewModel(CreateRoleResponse response)
        {
            RoleId = response.RoleId;
        }
    }
}
