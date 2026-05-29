namespace NetCore.DataAccess.DataObject.DTOs.Room
{
    public class RoomResponse
    {
        public int RoomID { get; set; }

        public int HotelID { get; set; }

        public string? RoomCode { get; set; }

        public int RoomSquare { get; set; }

        public bool IsActive { get; set; }
    }
}
