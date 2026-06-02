using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.DataAccess.DataObject.DTOs.Hotel
{
    public class HotelResponse
    {
        public int HotelID { get; set; }
        public string HotelName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
