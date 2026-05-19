using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetCore.DataAccess.DataObject;
using NetCore.DataAccess.IServices;

namespace NetCore.API.Controllers
{
    [Route("api/rooms")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private IRoomServices _roomServices;

        public RoomController(IRoomServices roomServices)
        {
            _roomServices = roomServices;
        }

        // GET: api/rooms
        [HttpGet]
        public async Task<IActionResult> GetRooms([FromQuery] RoomRequest request)
        {
            var result = await _roomServices.GetList(request);

            return Ok(result);
        }

        // POST: api/rooms
        [HttpPost]
        public async Task<IActionResult> CreateRoom([FromBody] RoomInsertRequest request)
        {
            var result = await _roomServices.Insert(request);

            if (result.ReturnCode > 0)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

    }
}
