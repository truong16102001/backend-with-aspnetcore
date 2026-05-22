using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NetCore.API.Filters
{
    public class PermissionAuthorizationFilter : IAsyncAuthorizationFilter
    {
        private readonly string _permission;
        public PermissionAuthorizationFilter(
           string permission)
        {
            _permission = permission;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // STEP 1:
            // Check user authenticated
            bool isAuthenticated =
                context.HttpContext.User.Identity
                    ?.IsAuthenticated ?? false;

            if (!isAuthenticated)
            {
                context.Result =
                    new UnauthorizedResult();

                return;
            }

            // STEP 2:
            // Get permission claims from JWT
            var permissions =
                context.HttpContext.User.Claims
                    .Where(x =>
                        x.Type == "permission")
                    .Select(x =>
                        x.Value)
                    .ToList();

            // STEP 3:
            // Check permission exists
            bool hasPermission =
                permissions.Contains(_permission);

            if (!hasPermission)
            {
                context.Result =
                    new ForbidResult();

                return;
            }

            await Task.CompletedTask;
        }
    }
}
