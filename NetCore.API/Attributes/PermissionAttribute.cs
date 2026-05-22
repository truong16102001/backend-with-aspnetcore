using Microsoft.AspNetCore.Mvc;
using NetCore.API.Filters;

namespace NetCore.API.Attributes
{
    public class PermissionAttribute : TypeFilterAttribute
    {
        public PermissionAttribute(string permission) : base(typeof(PermissionAuthorizationFilter))
        {
            Arguments = new object[]
            {
                permission
            };
        }
    }
}
