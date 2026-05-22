namespace NetCore.DataAccess.DataObject.Entities
{
    public class Hotel
    {
        public int HotelID { get; set; }

        public string HotelName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }

        public List<Room> Rooms { get; set; }
    }
}
