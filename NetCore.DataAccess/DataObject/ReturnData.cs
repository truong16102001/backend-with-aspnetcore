using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.DataAccess.DataObject
{
    public class ReturnData
    {
        public int ReturnCode {  get; set; }
        public string ReturnMsg { get; set; } = string.Empty;

    }
}
