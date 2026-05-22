using System.ComponentModel.DataAnnotations;

namespace NetCore.DataAccess.DataObject.Entities
{
    public class Feature
    {
        [Key]
        public int FeatureID { get; set; }

        public string FeatureCode { get; set; }

        public string FeatureName { get; set; }

        // Navigation
        public ICollection<UserPermission> UserPermissions { get; set; }
            = new List<UserPermission>();
    }
}
