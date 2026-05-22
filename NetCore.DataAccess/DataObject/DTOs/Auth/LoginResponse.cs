namespace NetCore.DataAccess.DataObject.DTOs.Auth
{
    public class LoginResponse
    {
        public string Token { get; set; }

        public DateTime ExpiredAt { get; set; }

        public string Username { get; set; }

        public string Fullname { get; set; }
    }
}
