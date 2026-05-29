using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.DataAccess.IServices
{
    public interface ICookieServices
    {
        void SetCookie(HttpResponse response, string key, string value, DateTime expires, SameSiteMode sameSite = SameSiteMode.Strict);

        void DeleteCookie(HttpResponse response, string key);
    }
}
