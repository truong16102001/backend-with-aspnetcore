using NetCore.DataAccess.DataObject;
using NetCore.DataAccess.DBContext;
using NetCore.DataAccess.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.DataAccess.Repositories
{
    public class RoomRepository : GenericRepository<Room>, IRoomRepository
    {
        public RoomRepository(MyDbContext dbContext)
           : base(dbContext)
        {

        }
    }
}
