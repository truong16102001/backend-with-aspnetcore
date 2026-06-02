using NetCore.DataAccess.DataObject.DTOs.Hotel;
using NetCore.DataAccess.DataObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.DataAccess.IRepositories
{
    public interface IHotelRepository
    {
        Task<List<HotelResponse>> GetListAsync(HotelRequest request);

        Task<int> CountAsync(HotelRequest request);

        Task<Hotel?> GetByIdAsync(int hotelId);

        Task<int> InsertAsync(CreateHotelRequest request);

        Task<int> UpdateAsync(int hotelId, UpdateHotelRequest request);
        Task<int> DeleteAsync(int hotelId);
    }
}
