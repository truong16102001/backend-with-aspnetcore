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
        public async Task<IActionResult> GetRooms(
            [FromQuery] RoomRequest request)
        {
            var result =
                await _roomServices.GetList(request);

            return Ok(result);
        }

        // GET: api/rooms/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoomById(
            int id)
        {
            var result =
                await _roomServices.GetById(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // POST: api/rooms
        [HttpPost]
        public async Task<IActionResult> CreateRoom(
            [FromBody] RoomInsertRequest request)
        {
            var result =
                await _roomServices.Insert(request);

            if (result.ReturnCode < 0)
            {
                return BadRequest(result);
            }

            return StatusCode(201, result);
        }

        // PUT: api/rooms/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoom(
            int id,
            [FromBody] RoomInsertRequest request)
        {
            var result =
                await _roomServices.Update(id, request);

            if (result.ReturnCode < 0)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        // DELETE: api/rooms/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(
            int id)
        {
            var result =
                await _roomServices.Delete(id);

            if (result.ReturnCode < 0)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

    }
}
