using NetCore.DataAccess.DataObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.DataAccess.IServices
{
    public interface IAccountService
    {
        Task<ReturnData> Login(AccountLoginDTO requestData);
    }
}
