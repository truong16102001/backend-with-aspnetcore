using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.DataAccess.DataObject
{
    public class AccountDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
    }

    public class AccountLoginDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
