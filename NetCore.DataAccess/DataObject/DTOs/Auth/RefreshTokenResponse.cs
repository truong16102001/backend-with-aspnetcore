using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.DataAccess.DataObject.DTOs.Auth
{
    public class RefreshTokenResponse
    {
        public string? AccessToken { get; set; }

        public DateTime AccessTokenExpiredAt { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime RefreshTokenExpiredAt { get; set; }
    }
}
