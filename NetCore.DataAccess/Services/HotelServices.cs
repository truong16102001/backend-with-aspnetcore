using System.Net;
using Microsoft.EntityFrameworkCore;
using NetCore.DataAccess.Common;
using NetCore.DataAccess.DataObject.Common;
using NetCore.DataAccess.DataObject.DTOs.Hotel;
using NetCore.DataAccess.DataObject.DTOs.Room;
using NetCore.DataAccess.DataObject.Entities;
using NetCore.DataAccess.IRepositories;
using NetCore.DataAccess.IServices;
using NetCore.DataAccess.UnitOfWork;

namespace NetCore.DataAccess.Services
{
    public class HotelServices : IHotelServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisServices _redis;
        private readonly IHotelRepository _hotelRepository;

        public HotelServices(IUnitOfWork unitOfWork, IRedisServices redis, IHotelRepository hotelRepository)
        {
            _unitOfWork = unitOfWork;
            _redis = redis;
            _hotelRepository = hotelRepository;
        }

        public async Task<ServiceResponse<PagedResponse<HotelResponse>>> GetList(HotelRequest request)
        {
            try
            {
                var cacheKey = CONSTANT.REDIS.HOTEL.LIST(request);

                // 1. Get on Redis
                var cached = await _redis.GetAsync<PagedResponse<HotelResponse>>(cacheKey);
                if (cached != null)
                {
                    return new ServiceResponse<PagedResponse<HotelResponse>>
                    {
                        Success = true,
                        StatusCode = (int)HttpStatusCode.OK,
                        Message = CONSTANT.MESSAGE.SUCCESS,
                        Data = cached
                    };
                }

                // 2. Cache miss → DB
                var hotels = await _hotelRepository.GetListAsync(request);

                // Count trước khi phân trang
                var totalCount = await _hotelRepository.CountAsync(request);

                var paged =
                    new PagedResponse<HotelResponse>
                    {
                        Items = hotels,
                        Page = request.PageNumber,
                        PageSize = request.PageSize,
                        TotalCount = totalCount
                    };

                // 3. Warm Redis
                await _redis.SetAsync(cacheKey, paged, CONSTANT.REDIS.CACHE_TTL.HOTEL);

                return new ServiceResponse<PagedResponse<HotelResponse>>
                {
                    Success = true,
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = CONSTANT.MESSAGE.SUCCESS,
                    Data = paged
                };
            }
            catch (Exception)
            {
                return new ServiceResponse<PagedResponse<HotelResponse>>
                {
                    Success = false,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = CONSTANT.MESSAGE.INTERNAL_ERROR,
                    Data = null
                };
            }
        }

        public async Task<ServiceResponse<HotelResponse>> GetById(int hotelId)
        {
            try
            {
                // 1. Get on Redis
                var cached = await _redis.GetAsync<HotelResponse>(CONSTANT.REDIS.HOTEL.DETAIL(hotelId));
                if (cached != null)
                {
                    return new ServiceResponse<HotelResponse>
                    {
                        Success = true,
                        StatusCode = (int)HttpStatusCode.OK,
                        Message = CONSTANT.MESSAGE.SUCCESS,
                        Data = cached
                    };
                }

                var hotel = await _hotelRepository.GetByIdAsync(hotelId);

                if (hotel == null)
                {
                    return new ServiceResponse<HotelResponse>
                    {
                        Success = false,
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = CONSTANT.MESSAGE.NOT_FOUND,
                        Data = null
                    };
                }

                var response = MapToResponse(hotel);

                // 3. Warm Redis
                await _redis.SetAsync(CONSTANT.REDIS.HOTEL.DETAIL(hotelId), response, CONSTANT.REDIS.CACHE_TTL.HOTEL);

                return new ServiceResponse<HotelResponse>
                {
                    Success = true,
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = CONSTANT.MESSAGE.SUCCESS,
                    Data = response
                };
            }
            catch (Exception)
            {
                return new ServiceResponse<HotelResponse>
                {
                    Success = false,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = CONSTANT.MESSAGE.INTERNAL_ERROR,
                    Data = null
                };
            }
        }

        public async Task<ServiceResponse<int>> Insert(CreateHotelRequest request)
        {
            try
            {
                int result = await _hotelRepository.InsertAsync(request);

                // List thay đổi (có item mới) → xóa toàn bộ list cache
                await _redis.RemoveByPrefixAsync(CONSTANT.REDIS.HOTEL.LIST_PREFIX());

                return new ServiceResponse<int>
                {
                    Success = true,
                    StatusCode = (int)HttpStatusCode.Created,
                    Message = CONSTANT.MESSAGE.CREATE_SUCCESS,
                    Data = result 
                };
            }
            catch (Exception)
            {
                return new ServiceResponse<int>
                {
                    Success = false,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = CONSTANT.MESSAGE.INTERNAL_ERROR,
                    Data = -1
                };
            }
        }

        public async Task<ServiceResponse<int>> Update(int hotelId, UpdateHotelRequest request)
        {
            try
            {
                var result = await _hotelRepository.UpdateAsync(hotelId, request);

                if (result > 0)
                {
                    // Song song: item đã thay đổi + list chứa item này
                    await Task.WhenAll(
                        _redis.RemoveAsync(CONSTANT.REDIS.HOTEL.DETAIL(hotelId)),
                        _redis.RemoveByPrefixAsync(CONSTANT.REDIS.HOTEL.LIST_PREFIX())
                    );
                }

                return new ServiceResponse<int>
                {
                    Success = true,
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = CONSTANT.MESSAGE.UPDATE_SUCCESS,
                    Data = result
                };
            }
            catch (Exception)
            {
                return new ServiceResponse<int>
                {
                    Success = false,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = CONSTANT.MESSAGE.INTERNAL_ERROR,
                    Data = -1
                };
            }
        }

        public async Task<ServiceResponse<bool>> Delete(int hotelId)
        {
            try
            {
                int result = await _hotelRepository.DeleteAsync(hotelId);

                if(result > 0)
                {
                    // Song song: item không còn tồn tại + list đã thay đổi
                    await Task.WhenAll(
                        _redis.RemoveAsync(CONSTANT.REDIS.HOTEL.DETAIL(hotelId)),
                        _redis.RemoveByPrefixAsync(CONSTANT.REDIS.HOTEL.LIST_PREFIX())
                    );
                }

                return new ServiceResponse<bool>
                {
                    Success = true,
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = CONSTANT.MESSAGE.DELETE_SUCCESS,
                    Data = result > 0
                };
            }
            catch (Exception)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = CONSTANT.MESSAGE.INTERNAL_ERROR,
                    Data = false
                };
            }
        }

        private HotelResponse MapToResponse(Hotel hotel)
        {
            return new HotelResponse
            {
                HotelName = hotel.HotelName,
                HotelID = hotel.HotelID,
                Description = hotel.Description!,
                CreatedDate = hotel.CreatedDate,
            };
        }
    }
}