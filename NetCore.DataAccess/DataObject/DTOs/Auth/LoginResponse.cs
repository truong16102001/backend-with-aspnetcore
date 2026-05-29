namespace NetCore.DataAccess.DataObject.DTOs.Auth
{
    public class LoginResponse
    {
        public string? AccessToken { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime AccessTokenExpiredAt { get; set; }

        public DateTime RefreshTokenExpiredAt { get; set; }

        public string? Username { get; set; }

        public string? Fullname { get; set; }

    }
}
