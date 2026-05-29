using Microsoft.EntityFrameworkCore;
using NetCore.DataAccess.Common;
using NetCore.DataAccess.DataObject.Common;
using NetCore.DataAccess.DataObject.DTOs.Auth;
using NetCore.DataAccess.DataObject.DTOs.Redis;
using NetCore.DataAccess.DataObject.Entities;
using NetCore.DataAccess.IServices;
using NetCore.DataAccess.UnitOfWork;
using System.Net;

namespace NetCore.DataAccess.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly ITokenServices _tokenServices;

        private readonly IRedisServices _redisServices;

        public AuthServices(
            IUnitOfWork unitOfWork,
            ITokenServices tokenServices, 
            IRedisServices redisServices)
        {
            _unitOfWork = unitOfWork;
            _tokenServices = tokenServices;
            _redisServices = redisServices;
        }

        public async Task<ServiceResponse<LoginResponse>> Login(LoginRequest request, DeviceInfo deviceInfo)
        {
            try
            {
                // STEP 1:
                // Find user by username
                var user = await _unitOfWork.Users
                    .Query()
                    .Include(x => x.UserPermissions)
                        .ThenInclude(x => x.Feature)
                    .Include(x => x.UserPermissions)
                        .ThenInclude(x => x.Permission)
                    .FirstOrDefaultAsync(x =>
                        x.Username == request.Username);

                // STEP 2:
                // Check user exists
                if (user == null)
                {
                    return new ServiceResponse<LoginResponse>
                    {
                        Success = false,
                        StatusCode = (int)HttpStatusCode.Unauthorized,
                        Message = "Invalid username or password",
                        Data = null
                    };
                }

                // STEP 3:
                // Compare raw password from request
                // with hashed password in database
                bool isValidPassword =
                    BCrypt.Net.BCrypt.Verify(
                        request.Password,
                        user.Password);

                if (!isValidPassword)
                {
                    return new ServiceResponse<LoginResponse>
                    {
                        Success = false,
                        StatusCode = (int)HttpStatusCode.Unauthorized,
                        Message = "Invalid username or password",
                        Data = null
                    };
                }

                string sid = Guid.NewGuid().ToString();

                _tokenServices.GenerateAccessToken(user, sid, out string accessToken, out DateTime accessTokenExpiredAt);

                // STEP 7: Create and refreshToken to db
                _tokenServices.GenerateRefreshToken(out string refreshToken, out DateTime refreshExpiredAt);

                string hashRt = _tokenServices.HashRefreshToken(refreshToken);

                var session = new UserSession { 
                    Sid = sid, 
                    UserID = user.UserID, 
                    RefreshTokenHash = hashRt, 
                    DeviceID = deviceInfo.DeviceID,
                    DeviceName = deviceInfo.DeviceName,
                    IPAddress = deviceInfo.IPAddress,
                    UserAgent = deviceInfo.UserAgent,
                    CreatedAt = DateTime.UtcNow, 
                    ExpiredAt = refreshExpiredAt, 
                    IsRevoked = false 
                };

                await _unitOfWork.UserSessions.Insert(session);

                await _unitOfWork.SaveChangesAsync();

                var permissions = user.UserPermissions.Select(x => $"{x.Feature.FeatureCode}.{x.Permission.PermissionCode}").ToList();

                var cachedSession = new CachedUserSession { 
                    UserId = user.UserID, 
                    Sid = sid, 
                    RefreshTokenHash = hashRt, 
                    Permissions = permissions, 
                    DeviceName = deviceInfo.DeviceName,
                    ExpiredAt = refreshExpiredAt 
                };

                TimeSpan ttl = refreshExpiredAt - DateTime.UtcNow;

                await _redisServices.SetSessionAsync(cachedSession, ttl);

                await _redisServices.SetRefreshTokenAsync(hashRt, sid, ttl);

                await _redisServices.AddUserSessionAsync(user.UserID, sid, ttl);

                var response = new LoginResponse
                {
                    AccessToken = accessToken,
                    AccessTokenExpiredAt = accessTokenExpiredAt,
                    RefreshToken = refreshToken,
                    RefreshTokenExpiredAt = refreshExpiredAt,
                    Username = user.Username,
                    Fullname = user.Fullname
                };

                return new ServiceResponse<LoginResponse>
                {
                    Success = true,
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = CONSTANT.MESSAGE.SUCCESS,
                    Data = response
                };
            }
            catch (Exception)
            {
                return new ServiceResponse<LoginResponse>
                {
                    Success = false,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = CONSTANT.MESSAGE.INTERNAL_ERROR,
                    Data = null
                };
            }
        }

        public async Task<ServiceResponse<RefreshTokenResponse>> RefreshToken(string refreshToken)
        {
            try
            {
                // =================================================
                // STEP 1:
                // HASH REFRESH TOKEN
                // =================================================
                string hashRt = _tokenServices.HashRefreshToken(refreshToken);

                if (string.IsNullOrWhiteSpace(refreshToken))
                {
                    return new ServiceResponse<RefreshTokenResponse>
                    {
                        Success = false,
                        StatusCode = (int)HttpStatusCode.Unauthorized,
                        Message = "Refresh token missing"
                    };
                }

                // =================================================
                // STEP 2:
                // LOOKUP REDIS
                //
                // auth:refresh_tokens:{hash_rt}
                // =================================================
                string? sid = await _redisServices.GetSessionIdByRefreshTokenAsync(hashRt);
                CachedUserSession? cachedSession = null;

                // =================================================
                // STEP 3:
                // REDIS HIT
                // =================================================
                if (!string.IsNullOrWhiteSpace(sid))
                {
                    cachedSession = await _redisServices.GetSessionAsync(sid);
                }

                // =================================================
                // STEP 4:
                // REDIS MISS
                //
                // ==> (**)
                // fallback DB
                // =================================================
                if (cachedSession == null)
                {
                    // =============================================
                    // GET VALID SESSION BY HASH_RT
                    // =============================================
                    var dbSession = await _unitOfWork.UserSessions.GetValidSessionByHashRtAsync(hashRt);

                    // =============================================
                    // CASE B2:
                    // SESSION NOT FOUND
                    // =============================================
                    if (dbSession == null)
                    {
                        return new ServiceResponse<RefreshTokenResponse>
                        {
                            Success = false,
                            StatusCode = (int)HttpStatusCode.Unauthorized,
                            Message = "Refresh token expired or revoked"
                        };
                    }

                    // =============================================
                    // CASE B1:
                    // REBUILD REDIS
                    // =============================================
                    var user = await _unitOfWork.Users.GetUserWithPermissionsAsync(dbSession.UserID);
                    if (user == null)
                    {
                        return new ServiceResponse<RefreshTokenResponse>
                        {
                            Success = false,
                            StatusCode = (int)HttpStatusCode.Unauthorized,
                            Message = "User not found"
                        };
                    }

                    var permissions = user.UserPermissions.Select(x => $"{x.Feature.FeatureCode}.{x.Permission.PermissionCode}").ToList();

                    cachedSession = new CachedUserSession
                    {
                        UserId = dbSession.UserID,
                        Sid = dbSession.Sid,
                        RefreshTokenHash = dbSession.RefreshTokenHash,
                        Permissions = permissions!,
                        DeviceName = dbSession.DeviceName,
                        ExpiredAt = dbSession.ExpiredAt
                    };

                    TimeSpan ttl = dbSession.ExpiredAt - DateTime.UtcNow;

                    // rebuild redis
                    await _redisServices.SetSessionAsync(cachedSession, ttl);
                    await _redisServices.SetRefreshTokenAsync(dbSession.RefreshTokenHash, dbSession.Sid, ttl);
                    await _redisServices.AddUserSessionAsync(dbSession.UserID, dbSession.Sid, ttl);
                    sid = dbSession.Sid;
                }

                // =================================================
                // STEP 5:
                // GENERATE NEW ACCESS TOKEN
                // =================================================
                var userEntity = await _unitOfWork.Users.GetById(cachedSession.UserId);
                if (userEntity == null)
                {
                    return new ServiceResponse<RefreshTokenResponse>
                    {
                        Success = false,
                        StatusCode = (int)HttpStatusCode.Unauthorized,
                        Message = "User not found"
                    };
                }

                _tokenServices.GenerateAccessToken(userEntity!, cachedSession.Sid!, out string newAccessToken, out DateTime accessTokenExpiredAt);

                // =================================================
                // STEP 6:
                // REFRESH TOKEN ROTATION
                // =================================================
                _tokenServices.GenerateRefreshToken(out string newRefreshToken, out DateTime refreshExpiredAt);
                string newHashRt = _tokenServices.HashRefreshToken(newRefreshToken);

                // =================================================
                // STEP 7:
                // UPDATE DB
                // =================================================
                var currentSession = await _unitOfWork.UserSessions.GetValidSessionBySidAsync(cachedSession.Sid!);
                if (currentSession == null)
                {
                    return new ServiceResponse<RefreshTokenResponse>
                    {
                        Success = false,
                        StatusCode = (int)HttpStatusCode.Unauthorized,
                        Message = "Session revoked"
                    };
                }

                currentSession!.RefreshTokenHash = newHashRt;
                currentSession.ExpiredAt = refreshExpiredAt;
                currentSession.LastActivityAt = DateTime.UtcNow;

                await _unitOfWork.SaveChangesAsync();

                // =================================================
                // STEP 8:
                // REMOVE OLD REDIS RT
                // =================================================
                await _redisServices.RemoveRefreshTokenAsync(cachedSession.RefreshTokenHash!);

                // =================================================
                // STEP 9:
                // UPDATE REDIS SESSION
                // =================================================
                cachedSession.RefreshTokenHash = newHashRt;
                cachedSession.ExpiredAt = refreshExpiredAt;
                TimeSpan newTtl = refreshExpiredAt - DateTime.UtcNow;
                await _redisServices.SetSessionAsync(cachedSession, newTtl);

                // =================================================
                // STEP 10:
                // ADD NEW RT MAPPING
                // =================================================
                await _redisServices.SetRefreshTokenAsync(newHashRt, cachedSession.Sid!, newTtl);

                // =================================================
                // RESPONSE
                // =================================================
                return new ServiceResponse<RefreshTokenResponse>
                {
                    Success = true,
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = "Refresh token success",
                    Data = new RefreshTokenResponse
                    {
                        AccessToken = newAccessToken,
                        AccessTokenExpiredAt = accessTokenExpiredAt,
                        RefreshToken = newRefreshToken,
                        RefreshTokenExpiredAt = refreshExpiredAt
                    }
                };
            }
            catch (Exception) {
                return new ServiceResponse<RefreshTokenResponse>
                {
                    Success = false,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = CONSTANT.MESSAGE.INTERNAL_ERROR,
                };
            }
        }

        public async Task<ServiceResponse<bool>> Register(RegisterRequest request)
        {
            try
            {
                var existedUser =
                    await _unitOfWork.Users.Query().FirstOrDefaultAsync(x => x.Username == request.Username);

                if (existedUser != null)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        StatusCode = (int)HttpStatusCode.Conflict,
                        Message = "Username already exists",
                        Data = false
                    };
                }

                string passwordHash =
                    BCrypt.Net.BCrypt.HashPassword(
                        request.Password);

                var user = new User
                {
                    Fullname = request.Fullname!,
                    Username = request.Username!,
                    Password = passwordHash,
                };

                await _unitOfWork.Users.Insert(user);

                await _unitOfWork.SaveChangesAsync();

                return new ServiceResponse<bool>
                {
                    Success = true,
                    StatusCode = (int)HttpStatusCode.Created,
                    Message = CONSTANT.MESSAGE.CREATE_SUCCESS,
                    Data = true
                };
            }
            catch (Exception)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = CONSTANT.MESSAGE.INTERNAL_ERROR,
                    Data = false
                };
            }
        }

        public async Task<ServiceResponse<bool>> Logout(string? refreshToken)
        {
            try
            {
                // =================================================
                // STEP 1:
                // CHECK NULL REFRESH TOKEN
                // =================================================

                if (string.IsNullOrWhiteSpace(refreshToken))
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = "Refresh token is required",
                        Data = false
                    };
                }

                // =================================================
                // STEP 2:
                // HASH REFRESH TOKEN
                // =================================================

                string hashRt =
                    _tokenServices
                        .HashRefreshToken(refreshToken);

                // =================================================
                // STEP 3:
                // GET SID FROM REDIS
                //
                // auth:refresh_tokens:{hash_rt}
                // =================================================

                string? sid =
                    await _redisServices
                        .GetSessionIdByRefreshTokenAsync(hashRt);

                UserSession? dbSession = null;

                // =================================================
                // STEP 4:
                // REDIS HIT
                // =================================================

                if (!string.IsNullOrWhiteSpace(sid))
                {
                    dbSession =
                        await _unitOfWork
                            .UserSessions
                            .GetValidSessionBySidAsync(sid);
                }

                // =================================================
                // STEP 5:
                // REDIS MISS
                //
                // fallback DB
                // =================================================

                if (dbSession == null)
                {
                    dbSession =
                        await _unitOfWork
                            .UserSessions
                            .GetValidSessionByHashRtAsync(hashRt);
                }

                // =================================================
                // STEP 6:
                // SESSION NOT FOUND
                // =================================================

                if (dbSession == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        StatusCode = (int)HttpStatusCode.Unauthorized,
                        Message = "Session not found",
                        Data = false
                    };
                }

                // =================================================
                // STEP 7:
                // SESSION ALREADY REVOKED
                // =================================================

                if (dbSession.IsRevoked)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = true,
                        StatusCode = (int)HttpStatusCode.OK,
                        Message = "Session already logged out",
                        Data = true
                    };
                }

                // =================================================
                // STEP 8:
                // REVOKE SESSION IN DB
                // =================================================

                dbSession.IsRevoked = true;

                dbSession.LastActivityAt = DateTime.UtcNow;

                await _unitOfWork.SaveChangesAsync();

                // =================================================
                // STEP 9:
                // REMOVE REDIS SESSION
                //
                // auth:sessions:{sid}
                // =================================================
                await _redisServices.RemoveSessionAsync(dbSession.Sid);

                // =================================================
                // STEP 10:
                // REMOVE REDIS REFRESH TOKEN
                //
                // auth:refresh_tokens:{hash_rt}
                // =================================================
                await _redisServices.RemoveRefreshTokenAsync(dbSession.RefreshTokenHash);

                // =================================================
                // STEP 11:
                // REMOVE USER SESSION
                //
                // auth:user_sessions:{user_id}
                // =================================================
                await _redisServices.RemoveUserSessionAsync(dbSession.UserID, dbSession.Sid);

                // =================================================
                // RESPONSE
                // =================================================
                return new ServiceResponse<bool>
                {
                    Success = true,
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = "Logout success",
                    Data = true
                };
            }
            catch (Exception)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = CONSTANT.MESSAGE.INTERNAL_ERROR,
                    Data = false
                };
            }
        }

        public async Task<ServiceResponse<bool>> LogoutAllDevices(string? refreshToken)
        {
            try
            {
                // =================================================
                // STEP 1:
                // CHECK NULL REFRESH TOKEN
                // =================================================

                if (string.IsNullOrWhiteSpace(refreshToken))
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = "Refresh token is required",
                        Data = false
                    };
                }

                // =================================================
                // STEP 2:
                // HASH REFRESH TOKEN
                // =================================================

                string hashRt =
                    _tokenServices
                        .HashRefreshToken(refreshToken);

                // =================================================
                // STEP 3:
                // FIND CURRENT SESSION
                // =================================================

                string? sid =
                    await _redisServices
                        .GetSessionIdByRefreshTokenAsync(hashRt);

                UserSession? currentSession = null;

                // =================================================
                // STEP 4:
                // REDIS HIT
                // =================================================

                if (!string.IsNullOrWhiteSpace(sid))
                {
                    currentSession =
                        await _unitOfWork
                            .UserSessions
                            .GetValidSessionBySidAsync(sid);
                }

                // =================================================
                // STEP 5:
                // REDIS MISS
                //
                // fallback DB
                // =================================================

                if (currentSession == null)
                {
                    currentSession =
                        await _unitOfWork
                            .UserSessions
                            .GetValidSessionByHashRtAsync(hashRt);
                }

                // =================================================
                // STEP 6:
                // SESSION NOT FOUND
                // =================================================

                if (currentSession == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        StatusCode = (int)HttpStatusCode.Unauthorized,
                        Message = "Session not found",
                        Data = false
                    };
                }

                // =================================================
                // STEP 7:
                // GET ALL ACTIVE SESSIONS OF USER
                // =================================================

                var userSessions =
                    await _unitOfWork
                        .UserSessions
                        .GetAllValidSessionsByUserIdAsync(
                            currentSession.UserID);

                // =================================================
                // STEP 8:
                // REVOKE ALL SESSIONS IN DB
                // =================================================

                foreach (var session in userSessions)
                {
                    session.IsRevoked = true;
                    session.LastActivityAt = DateTime.UtcNow;
                }

                await _unitOfWork.SaveChangesAsync();

                // =================================================
                // STEP 9:
                // REMOVE ALL REDIS CACHE
                // =================================================

                foreach (var session in userSessions)
                {
                    // =============================================
                    // REMOVE auth:sessions:{sid}
                    // =============================================
                    await _redisServices.RemoveSessionAsync(session.Sid);

                    // =============================================
                    // REMOVE auth:refresh_tokens:{hash_rt}
                    // =============================================
                    await _redisServices.RemoveRefreshTokenAsync(session.RefreshTokenHash);

                    // =============================================
                    // REMOVE auth:user_sessions:{user_id}
                    // =============================================
                    await _redisServices.RemoveUserSessionAsync(session.UserID, session.Sid);
                }

                return new ServiceResponse<bool>
                {
                    Success = true,
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = "Logout all devices success",
                    Data = true
                };
            }
            catch (Exception)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = CONSTANT.MESSAGE.INTERNAL_ERROR,
                    Data = false
                };
            }
        }
    }
}