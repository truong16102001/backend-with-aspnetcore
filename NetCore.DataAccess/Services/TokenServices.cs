using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NetCore.DataAccess.DataObject.Entities;
using NetCore.DataAccess.IServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace NetCore.DataAccess.Services
{
    public class TokenServices : ITokenServices
    {
        private readonly IConfiguration _configuration;
        public TokenServices(IConfiguration configuration) 
        { 
            _configuration = configuration; 
        }

        public ClaimsPrincipal? DecodeExpiredAccessToken(string accessToken)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, 
                ValidateIssuer = false, 
                ValidateIssuerSigningKey = true, 
                ValidateLifetime = false, 
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]!)) 
            };

            var tokenHandler = new JwtSecurityTokenHandler(); 
            var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out SecurityToken securityToken); 
            return principal;
        }

        public void GenerateAccessToken(User user, string sid, out string newAccessToken, out DateTime accessTokenExpiredAt)
        {
            //
            // Create claims for JWT token
            // =====================================================
            //
            // Claims = information stored inside token
            var claims = new List<Claim> { 
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), 
                new Claim(JwtRegisteredClaimNames.Sub, user.UserID.ToString()), 
                new Claim("sid", sid) 
            };
            // Create secret key for signing token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]!));
            accessTokenExpiredAt = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JWT:TokenValidityInMinutes"]));
            // Generate JWT token
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"], 
                audience: _configuration["JWT:ValidAudience"], 
                expires: accessTokenExpiredAt,
                claims: claims, 
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );
            newAccessToken = new JwtSecurityTokenHandler().WriteToken(token);
        }

        public void GenerateRefreshToken(out string token, out DateTime expiredAt)
        {
            byte[] randomBytes = RandomNumberGenerator.GetBytes(64);
            token = Convert.ToBase64String(randomBytes);
            expiredAt = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["JWT:RefreshTokenValidityInDays"]));
        }

        public string HashRefreshToken(string refreshToken)
        {
            using var sha256 = SHA256.Create(); 
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(refreshToken)); 
            return Convert.ToBase64String(bytes);
        }
    }
}
