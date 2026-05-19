using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.DataAccess.DataObject
{
    public class Room
    {
        [Key] // primary key
        public int RoomID {  get; set; }

        public int HotelID { get; set; }

        public string? RoomNumber { get; set; }

        public int RoomSquare {  get; set; }

        public int IsActive {  get; set; }

        public Hotel Hotel { get; set; }
    }

    public class RoomRequest
    {
        public string? RoomNumber { get; set; }

    }

    public class RoomInsertRequest
    {
        public int HotelID { get; set; }

        public string? RoomNumber { get; set; }

        public int RoomSquare { get; set; }

        public int IsActive { get; set; }

    }

    public class RoomResponse
    {
        public int RoomID { get; set; }

        public int HotelID { get; set; }

        public string RoomNumber { get; set; }

        public int RoomSquare { get; set; }

        public bool IsActive { get; set; }
    }
}
