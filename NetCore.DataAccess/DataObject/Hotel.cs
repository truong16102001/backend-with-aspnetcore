using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.DataAccess.DataObject
{
    public class Hotel
    {
        public int HotelID { get; set; }

        public string HotelName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate {  get; set; }

        public List<Room> Rooms { get; set; }
    }
}
