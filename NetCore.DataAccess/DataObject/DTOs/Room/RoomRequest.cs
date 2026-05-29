namespace NetCore.DataAccess.DataObject.DTOs.Room
{
    public class RoomRequest
    {
        public string? RoomCode { get; set; }

        // Paging — default page 1, pageSize 10
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
