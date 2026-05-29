using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.DataAccess.DataObject.DTOs.Auth
{
    public class DeviceInfo
    {
        public string? DeviceID { get; set; }

        public string? DeviceName { get; set; }

        public string? IPAddress { get; set; }

        public string? UserAgent { get; set; }
    }
}
