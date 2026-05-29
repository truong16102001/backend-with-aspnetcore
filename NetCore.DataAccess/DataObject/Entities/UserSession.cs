using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.DataAccess.DataObject.Entities
{
    public class UserSession { 
        public long UserSessionID { get; set; } 
        public string Sid { get; set; } 
        public int UserID { get; set; } 
        public string RefreshTokenHash { get; set; } 
        public string? DeviceID { get; set; } 
        public string? DeviceName { get; set; } 
        public string? IPAddress { get; set; } 
        public string? UserAgent { get; set; } 
        public bool IsRevoked { get; set; } 
        public DateTime CreatedAt { get; set; } 
        public DateTime? LastActivityAt { get; set; } 
        public DateTime ExpiredAt { get; set; } 
        public User User { get; set; } }
}
