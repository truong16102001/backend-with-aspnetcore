using Microsoft.EntityFrameworkCore;
using NetCore.DataAccess.Common;
using NetCore.DataAccess.DataObject.Common;
using NetCore.DataAccess.DataObject.DTOs.Room;
using NetCore.DataAccess.DataObject.Entities;
using NetCore.DataAccess.IServices;
using NetCore.DataAccess.UnitOfWork;
using System.Net;

namespace NetCore.DataAccess.Services
{
    public class RoomServices : IRoomServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisServices _redis;
        private static readonly TimeSpan CacheTTL = TimeSpan.FromMinutes(10);

        // Tách 2 loại key rõ ràng
        private static string RoomKey(int id) => $"rooms:{id}";
        private static string RoomListPrefix() => "rooms:list:";
        private static string RoomListKey(RoomRequest r) =>
            $"rooms:list:{r.RoomCode ?? "all"}:{r.PageNumber}:{r.PageSize}";

        public RoomServices(IUnitOfWork unitOfWork, IRedisServices redis)
        {
            _unitOfWork = unitOfWork;
            _redis = redis;
        }

        public async Task<ServiceResponse<RoomResponse>> Insert(CreateRoomRequest request)
        {
            try
            {
                var room = new Room
                {
                    HotelID = request.HotelID,
                    RoomCode = request.RoomNumber,
                    RoomSquare = request.RoomSquare,
                    IsActive = request.IsActive
                };

                await _unitOfWork.Rooms.Insert(room);

                await _unitOfWork.SaveChangesAsync();

                // List thay đổi (có item mới) → xóa toàn bộ list cache
                await _redis.RemoveByPrefixAsync(RoomListPrefix());

                return new ServiceResponse<RoomResponse>
                {
                    Success = true,
                    StatusCode = (int)HttpStatusCode.Created,
                    Message = CONSTANT.MESSAGE.CREATE_SUCCESS,
                    Data = MapToResponse(room)
                };
            }
            catch (Exception)
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

        public async Task<ServiceResponse<PagedResponse<RoomResponse>>> GetList(RoomRequest request)
        {
            try
            {
                var cacheKey = RoomListKey(request);

                // 1. Get on Redis
                var cached = await _redis.GetAsync<PagedResponse<RoomResponse>>(cacheKey);
                if (cached != null)
                {
                    return new ServiceResponse<PagedResponse<RoomResponse>>
                    {
                        Success = true,
                        StatusCode = (int)HttpStatusCode.OK,
                        Message = CONSTANT.MESSAGE.SUCCESS,
                        Data = cached
                    };
                }    

                // 2. Cache miss → DB
                var query = _unitOfWork.Rooms.Query();

                // Filter
                if (!string.IsNullOrWhiteSpace(request.RoomCode))
                    query = query.Where(r => r.RoomCode!.Contains(request.RoomCode));

                // Count trước khi phân trang
                var totalCount = await query.CountAsync();

                var rooms = await query
                    .OrderBy(r => r.RoomCode)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                var paged = new PagedResponse<RoomResponse>
                {
                    Data = rooms.Select(MapToResponse).ToList(),
                    Page = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalCount = totalCount
                };

                // 3. Warm Redis
                await _redis.SetAsync(cacheKey, paged, CacheTTL);

                return new ServiceResponse<PagedResponse<RoomResponse>>
                {
                    Success = true,
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = CONSTANT.MESSAGE.SUCCESS,
                    Data = paged
                };
            }
            catch (Exception)
            {
                return new ServiceResponse<PagedResponse<RoomResponse>>
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
                // 1. Get on Redis
                var cached = await _redis.GetAsync<RoomResponse>(RoomKey(id));
                if (cached != null)
                {
                    return new ServiceResponse<RoomResponse>
                    {
                        Success = true,
                        StatusCode = (int)HttpStatusCode.OK,
                        Message = CONSTANT.MESSAGE.SUCCESS,
                        Data = cached
                    };
                }

                var room = await _unitOfWork.Rooms.GetById(id);

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

                var response = MapToResponse(room);

                // 3. Warm Redis
                await _redis.SetAsync(RoomKey(id), response, CacheTTL);

                return new ServiceResponse<RoomResponse>
                {
                    Success = true,
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = CONSTANT.MESSAGE.SUCCESS,
                    Data = response
                };
            }
            catch (Exception)
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
                room.RoomCode = request.RoomNumber;
                room.RoomSquare = request.RoomSquare;
                room.IsActive = request.IsActive;

                _unitOfWork.Rooms.Update(room);

                await _unitOfWork.SaveChangesAsync();

                // Song song: item đã thay đổi + list chứa item này
                await Task.WhenAll(
                    _redis.RemoveAsync(RoomKey(id)),
                    _redis.RemoveByPrefixAsync(RoomListPrefix())
                );

                return new ServiceResponse<RoomResponse>
                {
                    Success = true,
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = CONSTANT.MESSAGE.UPDATE_SUCCESS,
                    Data = MapToResponse(room)
                };
            }
            catch (Exception)
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

                // Song song: item không còn tồn tại + list đã thay đổi
                await Task.WhenAll(
                    _redis.RemoveAsync(RoomKey(id)),
                    _redis.RemoveByPrefixAsync(RoomListPrefix())
                );

                return new ServiceResponse<bool>
                {
                    Success = true,
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = CONSTANT.MESSAGE.DELETE_SUCCESS,
                    Data = true
                };
            }
            catch (Exception)
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
                RoomCode = room.RoomCode!,
                RoomSquare = room.RoomSquare,
                IsActive = room.IsActive == 1
            };
        }

    }
}
