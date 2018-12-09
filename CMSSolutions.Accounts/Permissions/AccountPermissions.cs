using System.Collections.Generic;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.Security.Permissions;

namespace CMSSolutions.Accounts.Permissions
{
    [Feature(Constants.Areas.Accounts)]
    public class AccountPermissions : IPermissionProvider
    {
        public static readonly Permission ManageAccounts = new Permission
        {
            Name = "CMSSolutions.Accounts",
            Category = "System",
            Description = "Manage accounts"
        };

        public IEnumerable<Permission> GetPermissions()
        {
            yield return ManageAccounts;
        }   
    }
}