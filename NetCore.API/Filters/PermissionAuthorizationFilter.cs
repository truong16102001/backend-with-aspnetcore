using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NetCore.DataAccess.DataObject.DTOs.Redis;
using NetCore.DataAccess.IServices;
using NetCore.DataAccess.UnitOfWork;

namespace NetCore.API.Filters
{
    public class PermissionAuthorizationFilter : IAsyncAuthorizationFilter
    {
        private readonly string _permission;
        private readonly IRedisServices _redisServices;
        private readonly IUnitOfWork _unitOfWork;

        public PermissionAuthorizationFilter(
           string permission,
           IRedisServices redisServices,
            IUnitOfWork unitOfWork)
        {
            _permission = permission;
            _redisServices = redisServices;
            _unitOfWork = unitOfWork;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // =====================================================
            // STEP 1:
            // CHECK AUTHENTICATED
            // =====================================================
            bool isAuthenticated =
                context.HttpContext.User.Identity
                    ?.IsAuthenticated ?? false;

            if (!isAuthenticated)
            {
                context.Result =
                    new UnauthorizedResult();

                return;
            }

            // =====================================================
            // STEP 2:
            // GET SID FROM JWT
            // =====================================================
            string? sid =
                context.HttpContext.User
                    .FindFirst("sid")
                    ?.Value;

            if (string.IsNullOrWhiteSpace(sid))
            {
                context.Result =
                    new UnauthorizedResult();

                return;
            }

            // =====================================================
            // STEP 3:
            // GET SESSION FROM REDIS
            // =====================================================
            var cachedSession =
                await _redisServices
                    .GetSessionAsync(sid);


            // =====================================================
            // STEP 4:
            // REDIS MISS
            // (maybe redis restart / cache expired)
            // =====================================================
            if (cachedSession == null)
            {
                // =================================================
                // (*) FALLBACK DB
                //
                // Get user_session by:
                // - sid
                // - IsRevoked = false
                // - ExpiredAt > now
                // =================================================
                var dbSession =
                    await _unitOfWork.UserSessions
                        .GetValidSessionBySidAsync(sid);

                // =================================================
                // CASE B2:
                // SESSION NOT FOUND
                //
                // -> login again
                // =================================================
                if (dbSession == null)
                {
                    context.Result =
                        new UnauthorizedObjectResult(
                            new
                            {
                                Message =
                                    "Session expired or revoked"
                            });

                    return;
                }

                // =================================================
                // CASE B1:
                // SESSION FOUND
                //
                // -> rebuild redis cache
                // =================================================
                var user =
                    await _unitOfWork.Users
                        .GetUserWithPermissionsAsync(
                            dbSession.UserID);

                // =================================================
                // BUILD PERMISSIONS
                // =================================================
                var permissions =
                    user!.UserPermissions
                        .Select(x =>
                            $"{x.Feature.FeatureCode}.{x.Permission.PermissionCode}")
                        .ToList();

                // =================================================
                // BUILD REDIS SESSION DTO
                // =================================================
                cachedSession =
                    new CachedUserSession
                    {
                        UserId =
                            dbSession.UserID,

                        Sid =
                            dbSession.Sid,

                        RefreshTokenHash =
                            dbSession.RefreshTokenHash,

                        Permissions =
                            permissions,

                        DeviceName =
                            dbSession.DeviceName,

                        ExpiredAt =
                            dbSession.ExpiredAt
                    };

                // =================================================
                // REDIS TTL
                // =================================================
                TimeSpan ttl = dbSession.ExpiredAt - DateTime.UtcNow;

                // =================================================
                // REBUILD:
                // auth:sessions:{sid}
                // =================================================
                await _redisServices
                    .SetSessionAsync(
                        cachedSession,
                        ttl);

                // =================================================
                // REBUILD:
                // auth:refresh_tokens:{hash_rt}
                // =================================================
                await _redisServices
                    .SetRefreshTokenAsync(
                        dbSession.RefreshTokenHash,
                        dbSession.Sid,
                        ttl);

                // =================================================
                // REBUILD:
                // auth:user_sessions:{userId}
                // =================================================
                await _redisServices
                    .AddUserSessionAsync(
                        dbSession.UserID,
                        dbSession.Sid,
                        ttl);
            }

            // =====================================================
            // STEP 4:
            // FALLBACK DB IF REDIS LOST
            // =====================================================

            if (cachedSession == null)
            {
                context.Result =
                    new UnauthorizedResult();

                return;
            }

            // =====================================================
            // STEP 5:
            // CHECK PERMISSION
            // =====================================================
            bool hasPermission = cachedSession.Permissions.Contains(_permission);

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
