using NetCore.DataAccess.IRepositories;

namespace NetCore.DataAccess.UnitOfWork
{
    public interface IUnitOfWork
    {
        IRoomRepository Rooms { get; }

        IUserRepository Users { get; }

        Task<int> SaveChangesAsync();
    }
}
