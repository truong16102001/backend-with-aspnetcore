using NetCore.DataAccess.DataObject.Entities;

namespace NetCore.DataAccess.IRepositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetUserWithPermissionsAsync(int userId);
    }
}
