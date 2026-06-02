using Dapper;
using NetCore.DataAccess.Dapper;
using NetCore.DataAccess.DataObject.DTOs.Hotel;
using NetCore.DataAccess.DataObject.Entities;
using NetCore.DataAccess.DBContext;
using NetCore.DataAccess.IRepositories;

namespace NetCore.DataAccess.Repositories
{
    public class HotelRepository : GenericRepository<Hotel>, IHotelRepository
    {
        private readonly IApplicationDbConnection _db;

        public HotelRepository(MyDbContext dbContext, IApplicationDbConnection db)
          : base(dbContext)
        {
            _db = db;
        }

        public async Task<List<HotelResponse>> GetListAsync(HotelRequest request)
        {
            string sql = @"
                    SELECT
                        HotelID,
                        HotelName,
                        Description,
                        CreatedDate
                    FROM Hotels
                    WHERE
                    (
                        @HotelName IS NULL
                        OR HotelName LIKE '%' + @HotelName + '%'
                    )
                    ORDER BY HotelID DESC
                    OFFSET @Offset ROWS
                    FETCH NEXT @PageSize ROWS ONLY";

            return await _db.QueryAsync<HotelResponse>(
                sql,
                new
                {
                    request.HotelName,
                    Offset =
                        (request.PageNumber - 1)
                        * request.PageSize,
                    request.PageSize
                });
        }

        public async Task<int> CountAsync(HotelRequest request)
        {
            string sql = @"
                    SELECT COUNT(*)
                    FROM Hotels
                    WHERE
                    (
                        @HotelName IS NULL
                        OR HotelName LIKE '%' + @HotelName + '%'
                    )";

            return await _db.QuerySingleAsync<int>(
                sql,
                new
                {
                    request.HotelName
                });
        }

        public async Task<Hotel?> GetByIdAsync(int hotelId)
        {
            string sql = @"
                SELECT *
                FROM Hotels
                WHERE HotelID = @HotelID";

            return await _db.QueryFirstOrDefaultAsync<Hotel>(
                sql,
                new
                {
                    HotelID = hotelId
                });
        }

        public async Task<int> InsertAsync(CreateHotelRequest request)
        {
            string sql = @"
                INSERT INTO Hotels
                (
                    HotelName,
                    Description,
                    CreatedDate
                )
                VALUES
                (
                    @HotelName,
                    @Description,
                    GETDATE()
                )";

            return await _db.ExecuteAsync(
                sql,
                request);
        }

        public async Task<int> UpdateAsync(int hotelId, UpdateHotelRequest request)
        {
            string sql = @"
                UPDATE Hotels
                SET
                    HotelName = @HotelName,
                    Description = @Description
                WHERE HotelID = @HotelID";

            return await _db.ExecuteAsync(
                sql,
                new
                {
                    HotelID = hotelId,
                    request.HotelName,
                    request.Description
                });
        }

        public async Task<int> DeleteAsync(int hotelId)
        {
            string sql = @"
                DELETE FROM Hotels
                WHERE HotelID = @HotelID";

            return await _db.ExecuteAsync(
                sql,
                new
                {
                    HotelID = hotelId
                });
        }
    }
}