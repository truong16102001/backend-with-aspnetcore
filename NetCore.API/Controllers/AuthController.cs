using Microsoft.AspNetCore.Mvc;
using NetCore.API.Extensions;
using NetCore.DataAccess.DataObject.DTOs.Auth;
using NetCore.DataAccess.IServices;

namespace NetCore.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IAuthServices _authServices;
        private readonly ICookieServices _cookieServices;

        public AuthController(IAuthServices authServices, ICookieServices cookieServices)
        {
            _authServices = authServices;
            _cookieServices = cookieServices;
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // ======================================================
            // STEP 1:
            // Get device info from HttpContext
            // ======================================================
            var deviceInfo = HttpContext.GetDeviceInfo();

            // ======================================================
            // STEP 2:
            // Call auth service
            // ======================================================
            var result = await _authServices.Login(request, deviceInfo);

            // Login failed
            if (!result.Success || result.Data == null)
            {
                return StatusCode(
                    result.StatusCode,
                    result);
            }

            // ======================================================
            // STEP 4:
            // Set access token cookie
            // ======================================================
            _cookieServices.SetCookie(
                Response,
                "access_token",
                result.Data.AccessToken!,
                result.Data.AccessTokenExpiredAt);

            // ======================================================
            // STEP 5:
            // Set refresh token cookie
            // ======================================================
            _cookieServices.SetCookie(
                Response,
                "refresh_token",
                result.Data.RefreshToken!,
                result.Data.RefreshTokenExpiredAt);

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

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            string? refreshToken = Request.Cookies["refresh_token"];

            // ======================================================
            // STEP 2:
            // LOGOUT SERVICE
            // ======================================================
            var result = await _authServices.Logout(refreshToken);

            // ======================================================
            // DELETE ACCESS TOKEN COOKIE
            // ======================================================
            _cookieServices.DeleteCookie(
                Response,
                "access_token");

            // ======================================================
            // DELETE REFRESH TOKEN COOKIE
            // ======================================================
            _cookieServices.DeleteCookie(
                Response,
                "refresh_token");

            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("logout-all-devices")]
        public async Task<IActionResult>
    LogoutAllDevices()
        {
            // ======================================================
            // GET REFRESH TOKEN FROM COOKIE
            // ======================================================

            string? refreshToken =
                Request.Cookies["refresh_token"];

            // ======================================================
            // LOGOUT ALL DEVICES
            // ======================================================
            var result = await _authServices.LogoutAllDevices(refreshToken);

            // ======================================================
            // DELETE CURRENT DEVICE COOKIES
            // ======================================================

            _cookieServices.DeleteCookie(
                Response,
                "access_token");

            _cookieServices.DeleteCookie(
                Response,
                "refresh_token");

            return StatusCode(
                result.StatusCode,
                result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            // ======================================================
            // STEP 1:
            // Get refresh token from cookies
            // ======================================================
            string? refreshToken = Request.Cookies["refresh_token"];

            // ======================================================
            // STEP 2:
            // Refresh token
            // ======================================================
            var result = await _authServices.RefreshToken(refreshToken!);

            // ======================================================
            // STEP 3:
            // Refresh failed
            // ======================================================
            if (!result.Success ||
                result.Data == null)
            {
                _cookieServices.DeleteCookie(
                    Response,
                    "access_token");

                _cookieServices.DeleteCookie(
                    Response,
                    "refresh_token");

                return StatusCode(result.StatusCode, result);
            }

            // ======================================================
            // STEP 4:
            // Update cookies
            // ======================================================
            _cookieServices.SetCookie(
                Response,
                "access_token",
                result.Data.AccessToken!,
                result.Data.AccessTokenExpiredAt);

            _cookieServices.SetCookie(
                Response,
                "refresh_token",
                result.Data.RefreshToken!,
                result.Data.RefreshTokenExpiredAt);

            return StatusCode(result.StatusCode, result);
        }
    }
}
