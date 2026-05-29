using Microsoft.AspNetCore.Http;
using NetCore.DataAccess.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.DataAccess.Services
{
    public class CookieServices : ICookieServices
    {
        public void DeleteCookie(HttpResponse response, string key)
        {
            response.Cookies.Delete(key);
        }

        public void SetCookie(HttpResponse response, string key, string value, DateTime expires, SameSiteMode sameSite = SameSiteMode.Strict)
        {
            response.Cookies.Append(
                key,
                value,
                new CookieOptions
                {
                    // ======================================================
                    // Cookie expiration time
                    // ======================================================
                    Expires = expires,

                    // ======================================================
                    // Prevent javascript access
                    // (XSS protection)
                    // ======================================================
                    HttpOnly = true,

                    // ======================================================
                    // Only send through HTTPS
                    // ======================================================
                    Secure = true,

                    // ======================================================
                    // Required for authentication cookies
                    // ======================================================
                    IsEssential = true,

                    // ======================================================
                    // CSRF protection
                    // ======================================================
                    SameSite = sameSite
                });
        }
    }
}
