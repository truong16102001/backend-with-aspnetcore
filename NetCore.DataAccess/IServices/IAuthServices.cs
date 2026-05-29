using NetCore.DataAccess.DataObject.Common;
using NetCore.DataAccess.DataObject.DTOs.Auth;

namespace NetCore.DataAccess.IServices
{
    public interface IAuthServices
    {
        Task<ServiceResponse<LoginResponse>> Login(LoginRequest request, DeviceInfo deviceInfo);

        Task<ServiceResponse<bool>> Register(RegisterRequest request);

        Task<ServiceResponse<RefreshTokenResponse>> RefreshToken(string refreshToken);

        Task<ServiceResponse<bool>> Logout(string? refreshToken);

        Task<ServiceResponse<bool>> LogoutAllDevices(string? refreshToken);
    }
}
