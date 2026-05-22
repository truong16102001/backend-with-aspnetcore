using NetCore.DataAccess.DataObject.Common;
using NetCore.DataAccess.DataObject.DTOs.Room;
using NetCore.DataAccess.DataObject.Entities;

namespace NetCore.DataAccess.IServices
{
    public interface IRoomServices
    {
        Task<ServiceResponse<List<RoomResponse>>> GetList(RoomRequest request);

        Task<ServiceResponse<RoomResponse>> Insert(CreateRoomRequest request);

        Task<ServiceResponse<RoomResponse>> GetById(int id);

        Task<ServiceResponse<RoomResponse>> Update(int id, UpdateRoomRequest request);

        Task<ServiceResponse<bool>> Delete(int id);
    }
}
