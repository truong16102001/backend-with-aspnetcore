using System.ComponentModel.DataAnnotations;

namespace NetCore.DataAccess.DataObject.Entities
{
    public class UserPermission
    {
        [Key]
        public int UserPermissionID { get; set; }

        public int UserID { get; set; }

        public int FeatureID { get; set; }

        public int PermissionID { get; set; }

        // Navigation
        public User User { get; set; }

        public Feature Feature { get; set; }

        public Permission Permission { get; set; }
    }
}
