using NetCore.DataAccess.DBContext;
using NetCore.DataAccess.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.DataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MyDbContext _dbContext;

        public IRoomRepository Rooms { get; }
        public UnitOfWork(
            MyDbContext dbContext,
            IRoomRepository roomRepository)
        {
            _dbContext = dbContext;
            Rooms = roomRepository;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
