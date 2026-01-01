using System.Collections.Generic;

namespace Medinova.Areas.Admin.Data
{
    public class UserRoleViewModel
    {
        public class AssignRoleViewModel
        {
            public int UserId { get; set; }
            public string FullName { get; set; }
            public List<RoleSelection> Roles { get; set; }
        }

        public class RoleSelection
        {
            public int RoleId { get; set; }
            public string RoleName { get; set; }
            public bool IsSelected { get; set; } // Kullanıcı bu role sahip mi?
        }
    }
}