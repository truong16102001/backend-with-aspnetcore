namespace NetCore.DataAccess.DataObject.DTOs.Redis
{
    public class CachedUserSession { 
        public int UserId { get; set; } 

        public string? Sid { get; set; } 

        public string? RefreshTokenHash { get; set; } 

        public List<string>? Permissions { get; set; } 

        public string? DeviceName { get; set; } 

        public DateTime ExpiredAt { get; set; } 
    }
}
