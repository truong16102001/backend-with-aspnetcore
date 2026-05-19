using Microsoft.EntityFrameworkCore;
using NetCore.DataAccess.DataObject;
using NetCore.DataAccess.DBContext;
using NetCore.DataAccess.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.DataAccess.Services
{
    public class RoomServices : IRoomServices
    {
        MyDbContext _dbContext;

        public RoomServices(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<RoomResponse>> GetList(RoomRequest roomsRequest)
        {
            var query = _dbContext.Rooms.AsQueryable();

            if (!string.IsNullOrEmpty(roomsRequest.RoomNumber))
            {
                query = query.Where(r =>
                    r.RoomNumber.Contains(roomsRequest.RoomNumber));
            }

            var result = await query
                .Select(r => new RoomResponse
                {
                    RoomID = r.RoomID,
                    HotelID = r.HotelID,
                    RoomNumber = r.RoomNumber,
                    RoomSquare = r.RoomSquare,
                    IsActive = r.IsActive == 1
                })
                .ToListAsync();

            return result;
        }

        public async Task<ReturnData> Insert(RoomInsertRequest roomInsertRequest)
        {
            var result = new ReturnData();

            try
            {
                // Create new room object
                var room = new Room()
                {
                    HotelID = roomInsertRequest.HotelID,
                    RoomNumber = roomInsertRequest.RoomNumber,
                    RoomSquare = roomInsertRequest.RoomSquare,
                    IsActive = roomInsertRequest.IsActive
                };

                // Add to database
                await _dbContext.Rooms.AddAsync(room);

                // Save changes
                await _dbContext.SaveChangesAsync();

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
    }
}
