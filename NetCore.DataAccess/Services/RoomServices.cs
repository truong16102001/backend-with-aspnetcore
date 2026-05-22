using Microsoft.EntityFrameworkCore;
using NetCore.DataAccess.Common;
using NetCore.DataAccess.DataObject.Common;
using NetCore.DataAccess.DataObject.DTOs;
using NetCore.DataAccess.DataObject.DTOs.Room;
using NetCore.DataAccess.DataObject.Entities;
using NetCore.DataAccess.IServices;
using NetCore.DataAccess.UnitOfWork;
using System.Net;
using static NetCore.DataAccess.Common.CONSTANT;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NetCore.DataAccess.Services
{
    public class RoomServices : IRoomServices
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoomServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResponse<RoomResponse>> Insert(CreateRoomRequest request)
        {
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

                return new ServiceResponse<RoomResponse>
                {
                    Success = true,
                    StatusCode = (int)HttpStatusCode.Created,
                    Message = CONSTANT.MESSAGE.CREATE_SUCCESS,
                    Data = MapToResponse(room)
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<RoomResponse>
                {
                    Success = false,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = CONSTANT.MESSAGE.INTERNAL_ERROR,
                    Data =null
                };
            }
        }

        public async Task<ServiceResponse<List<RoomResponse>>> GetList(RoomRequest request)
        {
            try
            {
                var query = _unitOfWork.Rooms.Query();

                // Filter
                if (!string.IsNullOrWhiteSpace(
                    request.RoomNumber))
                {
                    query = query.Where(r =>
                        r.RoomNumber.Contains(
                            request.RoomNumber));
                }

                // Sort
                query = query.OrderBy(r => r.RoomNumber);

                var rooms = await query.ToListAsync();

                var result = rooms
                    .Select(MapToResponse)
                    .ToList();

                return new ServiceResponse<List<RoomResponse>>
                {
                    Success = true,
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = CONSTANT.MESSAGE.SUCCESS,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<RoomResponse>>
                {
                    Success = false,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = CONSTANT.MESSAGE.INTERNAL_ERROR,
                    Data = null
                };
            }
        }

        public async Task<ServiceResponse<RoomResponse>> GetById(int id)
        {
            try
            {
                var room =
                    await _unitOfWork.Rooms.GetById(id);

                if (room == null)
                {
                    return new ServiceResponse<RoomResponse>
                    {
                        Success = false,
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = CONSTANT.MESSAGE.NOT_FOUND,
                        Data = null
                    };
                }

                return new ServiceResponse<RoomResponse>
                {
                    Success = true,
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = CONSTANT.MESSAGE.SUCCESS,
                    Data = MapToResponse(room)
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<RoomResponse>
                {
                    Success = false,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = CONSTANT.MESSAGE.INTERNAL_ERROR,
                    Data = null
                };
            }
        }

        public async Task<ServiceResponse<RoomResponse>> Update(int id, UpdateRoomRequest request)
        {
            try
            {
                var room =
                    await _unitOfWork.Rooms.GetById(id);

                if (room == null)
                {
                    return new ServiceResponse<RoomResponse>
                    {
                        Success = false,
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = CONSTANT.MESSAGE.NOT_FOUND,
                        Data = null
                    };
                }

                room.HotelID = request.HotelID;
                room.RoomNumber = request.RoomNumber;
                room.RoomSquare = request.RoomSquare;
                room.IsActive = request.IsActive;

                _unitOfWork.Rooms.Update(room);

                await _unitOfWork.SaveChangesAsync();

                return new ServiceResponse<RoomResponse>
                {
                    Success = true,
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = CONSTANT.MESSAGE.UPDATE_SUCCESS,
                    Data = MapToResponse(room)
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<RoomResponse>
                {
                    Success = false,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = CONSTANT.MESSAGE.INTERNAL_ERROR,
                    Data = null
                };
            }
        }

        public async Task<ServiceResponse<bool>> Delete(int id)
        {
            try
            {
                var room = await _unitOfWork.Rooms.GetById(id);

                if (room == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = CONSTANT.MESSAGE.NOT_FOUND,
                        Data = default
                    };
                }

                _unitOfWork.Rooms.Delete(room);

                await _unitOfWork.SaveChangesAsync();

                return new ServiceResponse<bool>
                {
                    Success = true,
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = CONSTANT.MESSAGE.DELETE_SUCCESS,
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = CONSTANT.MESSAGE.INTERNAL_ERROR,
                    Data = default
                };
            }
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
