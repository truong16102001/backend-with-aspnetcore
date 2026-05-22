using System.ComponentModel.DataAnnotations;

namespace NetCore.DataAccess.DataObject.Entities
{
    public class Permission
    {
        [Key]
        public int PermissionID { get; set; }

        public string PermissionCode { get; set; }

        public string PermissionName { get; set; }

        // Navigation
        public ICollection<UserPermission> UserPermissions { get; set; }
            = new List<UserPermission>();
    }
}
