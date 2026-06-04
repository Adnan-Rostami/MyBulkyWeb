using Microsoft.AspNetCore.Identity;

namespace BulkyWeb.Models.Permissions
{
    public class RolePermission
    {
        public string RoleID { get; set; }
        public int PermissionID { get; set; }

        public IdentityRole Role { get; set; }
        public Permission Permission { get; set; } = null!;
        //public IdentityRole Role { get; set; } = null!;
        // public ApplicationRole Role { get; set; } = null!;

    }

}
