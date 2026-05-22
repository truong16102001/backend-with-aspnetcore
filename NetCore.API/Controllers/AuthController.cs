using Microsoft.AspNetCore.Mvc;
using NetCore.DataAccess.DataObject.DTOs.Auth;
using NetCore.DataAccess.IServices;

namespace NetCore.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IAuthServices _authServices;
        public AuthController(IAuthServices authServices)
        {
            _authServices = authServices;
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authServices.Login(request);
            // Login failed
            if (!result.Success || result.Data == null)
            {
                return StatusCode(
                    result.StatusCode,
                    result);
            }
            // ======================================================
            // SET ACCESS TOKEN COOKIE
            // ======================================================
            Response.Cookies.Append(
                "access_token",
                result.Data.Token,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,

                    Expires =
                        result.Data.ExpiredAt
                });

            return StatusCode(result.StatusCode, result);
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterRequest request)
        {
            var result =
               await _authServices.Register(request);

            return StatusCode(
                result.StatusCode,
                result);
        }
    }
}
