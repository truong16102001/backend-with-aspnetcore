using NetCore.DataAccess.IRepositories;

namespace NetCore.DataAccess.UnitOfWork
{
    public interface IUnitOfWork
    {
        IRoomRepository Rooms { get; }

        IUserRepository Users { get; }

        IUserSessionRepository UserSessions { get; }

        IHotelRepository Hotels { get; }

        Task<int> SaveChangesAsync();
    }
}
