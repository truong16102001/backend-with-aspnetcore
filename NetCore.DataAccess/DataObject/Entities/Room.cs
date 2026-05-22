using System.ComponentModel.DataAnnotations;

namespace NetCore.DataAccess.DataObject.Entities
{
    public class Room
    {
        [Key] // primary key
        public int RoomID { get; set; }

        public int HotelID { get; set; }

        public string? RoomNumber { get; set; }

        public int RoomSquare { get; set; }

        public int IsActive { get; set; }

        public Hotel Hotel { get; set; }
    }

    public class RoomRequest
    {
        public string? RoomNumber { get; set; }

    }

}
