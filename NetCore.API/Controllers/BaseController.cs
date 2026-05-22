using Microsoft.AspNetCore.Mvc;
using NetCore.DataAccess.DataObject.Common;

namespace NetCore.API.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected IActionResult Success<T>(
            T data,
            int statusCode = 200,
            string message = "Success")
        {
            return Ok(new APIResponse<T>
            {
                StatusCode = statusCode,
                Message = message,
                Data = data
            });
        }

        protected IActionResult Error(
            string message,
            int statusCode = 400)
        {
            return StatusCode(statusCode,
                new APIResponse<object>
                {
                    StatusCode = statusCode,
                    Message = message,
                    Data = null
                });
        }
    }
}
