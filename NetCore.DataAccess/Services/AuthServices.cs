using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NetCore.DataAccess.Common;
using NetCore.DataAccess.DataObject.Common;
using NetCore.DataAccess.DataObject.DTOs;
using NetCore.DataAccess.DataObject.DTOs.Auth;
using NetCore.DataAccess.DataObject.Entities;
using NetCore.DataAccess.IServices;
using NetCore.DataAccess.UnitOfWork;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace NetCore.DataAccess.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IConfiguration _configuration;

        public AuthServices(
            IUnitOfWork unitOfWork,
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<ServiceResponse<LoginResponse>> Login(LoginRequest request)
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

                // STEP 4:
                // Create claims for JWT token
                // =====================================================
                //
                // Claims = information stored inside token
                //
                // Example:
                // - username, userid,  role, email
                var authClaims = new List<Claim>
                {
                    new Claim(
                        ClaimTypes.Name,
                        user.Username),

                    new Claim(
                        ClaimTypes.NameIdentifier,
                        user.UserID.ToString()),

                    new Claim(
                        JwtRegisteredClaimNames.Jti,
                        Guid.NewGuid().ToString()),
                };

                foreach (var item in user.UserPermissions)
                {
                    string permissionValue =
                        $"{item.Feature.FeatureCode}.{item.Permission.PermissionCode}";

                    authClaims.Add(
                        new Claim(
                            "permission",
                            permissionValue));
                }

                // STEP 5:
                // Create secret key for signing token
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));

                // STEP 6:
                // Generate JWT token
                var token = new JwtSecurityToken(
                    issuer:
                        _configuration["JWT:ValidIssuer"],

                    audience:
                        _configuration["JWT:ValidAudience"],

                    expires:
                        DateTime.Now.AddMinutes(
                            Convert.ToDouble(
                                _configuration[
                                    "JWT:TokenValidityInMinutes"])),

                    claims:
                        authClaims,

                    signingCredentials:
                        new SigningCredentials(
                            authSigningKey,
                            SecurityAlgorithms.HmacSha256)
                );

                //STEP 7: Create and refreshToken to db

                var response = new LoginResponse
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    ExpiredAt = token.ValidTo,
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
            catch (Exception ex)
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
                    Fullname = request.Fullname,
                    Username = request.Username,
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
            catch (Exception ex)
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