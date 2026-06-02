using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.DataAccess.DataObject.DTOs.Hotel
{
    public class HotelRequest
    {
        public string? HotelName { get; set; }  // filter contains
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
