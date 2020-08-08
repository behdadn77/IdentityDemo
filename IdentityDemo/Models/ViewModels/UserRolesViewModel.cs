using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemo.Models.ViewModels
{
    public class UserRolesViewModel
    {
        public ApplicationUser User { get; set; }
        public List<UserRoleStatus> Roles { get; set; } = new List<UserRoleStatus>();
    }

    public class UserRoleStatus
    {
        public string RoleName { get; set; }
        public bool IsInRole { get; set; }
    }
}
