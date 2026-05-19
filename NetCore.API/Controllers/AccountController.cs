using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using NetCore.DataAccess.DataObject;
using NetCore.DataAccess.IServices;

namespace NetCore.API.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IAccountService _accountService; // AccountController giao tiếp với AccountService thông qua interface chứ không phụ thuộc trực tiếp
        public AccountController(IAccountService accountService) // dependency injection by constructor
        {
            _accountService = accountService;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(AccountLoginDTO loginRequest)
        {
            var returnData = new ReturnData();
            try
            {
                returnData = await _accountService.Login(loginRequest);
                return Ok(returnData);
            }
            catch (Exception ex) {
                throw;
            }
        }
    }
}
