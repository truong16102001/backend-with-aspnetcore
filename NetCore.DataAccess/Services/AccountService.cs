using NetCore.DataAccess.DataObject;
using NetCore.DataAccess.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.DataAccess.Services
{
    public class AccountService : IAccountService
    {
        public async Task<ReturnData> Login(AccountLoginDTO requestData)
        {
            var returnData = new ReturnData();
            try
            {
                if (requestData == null 
                    || string.IsNullOrEmpty(requestData.UserName)
                    || string.IsNullOrEmpty(requestData.Password)) {
                    returnData.ReturnCode = -1;
                    returnData.ReturnMsg = "Invalid data";
                }
                else
                {
                    returnData.ReturnCode = 1;
                    returnData.ReturnMsg = "Login successfully";
                }
                return returnData;
            }
            catch (Exception ex) {
                throw;
            }
        }
    }
}
