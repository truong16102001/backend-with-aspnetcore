using NetCore.DataAccess.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.DataAccess.UnitOfWork
{
    public interface IUnitOfWork
    {
        IRoomRepository Rooms { get; }

        Task<int> SaveChangesAsync();
    }
}
