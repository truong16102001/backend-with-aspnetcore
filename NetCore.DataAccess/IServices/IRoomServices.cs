using NetCore.DataAccess.DataObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.DataAccess.IServices
{
    public interface IRoomServices
    {
        Task<List<RoomResponse>> GetList(RoomRequest roomsRequest);

        Task<ReturnData> Insert(RoomInsertRequest roomInsertRequest);

        Task<RoomResponse?> GetById(int id);

        Task<ReturnData> Update(int id, RoomInsertRequest request);

        Task<ReturnData> Delete(int id);
    }
}
