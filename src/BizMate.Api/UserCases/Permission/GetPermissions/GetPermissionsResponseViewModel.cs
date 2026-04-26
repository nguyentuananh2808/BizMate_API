using BizMate.Application.UserCases.RoleAggregate.Permission.Queries.GetPermissions;

namespace BizMate.Api.UserCases.Permission.GetPermissions
{
    public class GetPermissionsResponseViewModel
    {
        public List<PermissionGroupDto> Groups { get; set; }

        public GetPermissionsResponseViewModel(GetPermissionsResponse response)
        {
            Groups = response.Groups;
        }
    }
}
