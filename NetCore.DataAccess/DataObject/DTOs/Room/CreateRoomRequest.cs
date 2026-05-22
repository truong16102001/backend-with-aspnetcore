namespace NetCore.DataAccess.DataObject.DTOs.Room
{
    public class CreateRoomRequest
    {
        public int HotelID { get; set; }

        public string? RoomNumber { get; set; }

        public int RoomSquare { get; set; }

        public int IsActive { get; set; }
    }
}
