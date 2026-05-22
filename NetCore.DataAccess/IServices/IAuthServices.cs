using NetCore.DataAccess.DataObject.Common;
using NetCore.DataAccess.DataObject.DTOs.Auth;

namespace NetCore.DataAccess.IServices
{
    public interface IAuthServices
    {
        Task<ServiceResponse<LoginResponse>> Login(LoginRequest request);
        Task<ServiceResponse<bool>> Register(RegisterRequest request);

    }
}
