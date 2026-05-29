using NetCore.DataAccess.DBContext;
using NetCore.DataAccess.IRepositories;

namespace NetCore.DataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MyDbContext _dbContext;

        public IRoomRepository Rooms { get; }

        public IUserRepository Users { get; }

        public IUserSessionRepository UserSessions {  get; }

        public UnitOfWork(
            MyDbContext dbContext,
            IRoomRepository roomRepository, IUserRepository userRepository, IUserSessionRepository userSessions)
        {
            _dbContext = dbContext;
            Rooms = roomRepository;
            Users = userRepository;
            UserSessions = userSessions;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
