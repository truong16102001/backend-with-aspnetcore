using NetCore.DataAccess.DataObject.Common;
using NetCore.DataAccess.DataObject.DTOs.Hotel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.DataAccess.IServices
{
    public interface IHotelServices
    {
        Task<ServiceResponse<PagedResponse<HotelResponse>>> GetList(HotelRequest request);

        Task<ServiceResponse<HotelResponse>> GetById(int hotelId);

        Task<ServiceResponse<int>> Insert(CreateHotelRequest request);

        Task<ServiceResponse<int>> Update(int hotelId, UpdateHotelRequest request);

        Task<ServiceResponse<bool>> Delete(int hotelId);
    }
}
