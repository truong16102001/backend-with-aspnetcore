using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.DataAccess.Common
{
    public class DatabaseOptions
    {
        public string Provider { get; set; } = string.Empty;

        public string ConnectionString { get; set; } = string.Empty;
    }
}
