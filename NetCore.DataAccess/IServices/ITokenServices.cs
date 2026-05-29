using NetCore.DataAccess.DataObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.DataAccess.IServices
{
    public interface ITokenServices
    {
        void GenerateAccessToken(User user, string sid, out string newAccessToken, out DateTime accessTokenExpiredAt); 

        void GenerateRefreshToken(out string token, out DateTime expiredAt);

        string HashRefreshToken(string refreshToken); 

        ClaimsPrincipal? DecodeExpiredAccessToken(string accessToken);
    }
}
