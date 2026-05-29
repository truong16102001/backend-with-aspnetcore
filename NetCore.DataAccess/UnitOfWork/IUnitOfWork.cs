using NetCore.DataAccess.IRepositories;

namespace NetCore.DataAccess.UnitOfWork
{
    public interface IUnitOfWork
    {
        IRoomRepository Rooms { get; }

        IUserRepository Users { get; }

        IUserSessionRepository UserSessions { get; }

        Task<int> SaveChangesAsync();
    }
}
