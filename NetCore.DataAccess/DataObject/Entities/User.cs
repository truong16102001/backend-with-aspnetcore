using System.ComponentModel.DataAnnotations;

namespace NetCore.DataAccess.DataObject.Entities
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        public string Fullname { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        // Navigation
        public ICollection<UserPermission> UserPermissions { get; set; }
            = new List<UserPermission>();
    }
}
