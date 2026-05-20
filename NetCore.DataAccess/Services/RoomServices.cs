using Azure.Core;
using Microsoft.EntityFrameworkCore;
using NetCore.DataAccess.DataObject;
using NetCore.DataAccess.DBContext;
using NetCore.DataAccess.IRepositories;
using NetCore.DataAccess.IServices;
using NetCore.DataAccess.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.DataAccess.Services
{
    public class RoomServices : IRoomServices
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoomServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ReturnData> Insert(RoomInsertRequest request)
        {
            var result = new ReturnData();

            try
            {
                var room = new Room
                {
                    HotelID = request.HotelID,
                    RoomNumber = request.RoomNumber,
                    RoomSquare = request.RoomSquare,
                    IsActive = request.IsActive
                };

                await _unitOfWork.Rooms.Insert(room);

                await _unitOfWork.SaveChangesAsync();

                result.ReturnCode = 1;
                result.ReturnMsg = "Insert successful";
            }
            catch (Exception ex)
            {
                result.ReturnCode = -1;
                result.ReturnMsg = ex.Message;
            }

            return result;
        }

        public async Task<List<RoomResponse>> GetList(RoomRequest roomsRequest)
        {
            var query = _unitOfWork.Rooms.Query();

            // Filter
            if (!string.IsNullOrEmpty(
                roomsRequest.RoomNumber))
            {
                query = query.Where(r =>
                    r.RoomNumber.Contains(
                        roomsRequest.RoomNumber));
            }

            // Sort example
            query = query.OrderBy(r => r.RoomNumber);

            // Execute SQL here
            var rooms = await query.ToListAsync();

            // Mapping
            return rooms.Select(r => new RoomResponse
            {
                RoomID = r.RoomID,
                HotelID = r.HotelID,
                RoomNumber = r.RoomNumber,
                RoomSquare = r.RoomSquare,
                IsActive = r.IsActive == 1
            }).ToList();
        }

        public async Task<RoomResponse?> GetById(int id)
        {
            var room = await _unitOfWork.Rooms.GetById(id);
            if (room == null)
            {
                return null;
            }
            return MapToResponse(room);
        }

        public async Task<ReturnData> Update(int id, RoomInsertRequest request)
        {
            var result = new ReturnData();
            try
            {
                var room = await _unitOfWork.Rooms.GetById(id);
                if (room == null)
                {
                    result.ReturnCode = -1;
                    result.ReturnMsg = "Room not found";

                    return result;
                }
                room.HotelID = request.HotelID;
                room.RoomNumber = request.RoomNumber;
                room.RoomSquare = request.RoomSquare;
                room.IsActive = request.IsActive;
                _unitOfWork.Rooms.Update(room);
                await _unitOfWork.SaveChangesAsync();
                result.ReturnCode = 1;
                result.ReturnMsg = "Update successful";
            }
            catch (Exception ex) {
                result.ReturnCode = -1;
                result.ReturnMsg = ex.Message;
            }
            return result;
        }

        public async Task<ReturnData> Delete(int id)
        {
            var result = new ReturnData();

            try
            {
                var room = await _unitOfWork.Rooms.GetById(id);

                if (room == null)
                {
                    result.ReturnCode = -1;
                    result.ReturnMsg = "Room not found";

                    return result;
                }

                _unitOfWork.Rooms.Delete(room);

                await _unitOfWork.SaveChangesAsync();

                result.ReturnCode = 1;
                result.ReturnMsg = "Delete successful";
            }
            catch (Exception ex)
            {
                result.ReturnCode = -1;
                result.ReturnMsg = ex.Message;
            }

            return result;
        }

        private RoomResponse MapToResponse(Room room)
        {
            return new RoomResponse
            {
                RoomID = room.RoomID,
                HotelID = room.HotelID,
                RoomNumber = room.RoomNumber,
                RoomSquare = room.RoomSquare,
                IsActive = room.IsActive == 1
            };
        }
    }
}
