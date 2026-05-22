using NetCore.DataAccess.DBContext;
using NetCore.DataAccess.IRepositories;

namespace NetCore.DataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MyDbContext _dbContext;

        public IRoomRepository Rooms { get; }

        public IUserRepository Users { get; }

        public UnitOfWork(
            MyDbContext dbContext,
            IRoomRepository roomRepository, IUserRepository userRepository)
        {
            _dbContext = dbContext;
            Rooms = roomRepository;
            Users = userRepository;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
