using Microsoft.AspNetCore.Mvc;
using NetCore.API.Attributes;
using NetCore.DataAccess.Common;
using NetCore.DataAccess.DataObject.DTOs.Room;
using NetCore.DataAccess.DataObject.Entities;
using NetCore.DataAccess.IServices;

namespace NetCore.API.Controllers
{
    [Route("api/rooms")]
    [ApiController]
    public class RoomController : BaseController
    {
        private IRoomServices _roomServices;

        public RoomController(IRoomServices roomServices)
        {
            _roomServices = roomServices;
        }

        // GET: api/rooms
        [Permission(CONSTANT.PERMISSION.ROOM.READ)]
        [HttpGet]
        public async Task<IActionResult> GetRooms(
            [FromQuery] RoomRequest request)
        {
            var serviceResponse = await _roomServices.GetList(request);
            return StatusCode(serviceResponse.StatusCode, serviceResponse);
        }

        // GET: api/rooms/1
        [Permission(CONSTANT.PERMISSION.ROOM.READ)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoomById(
            int id)
        {
            var serviceResponse = await _roomServices.GetById(id);
            return StatusCode(serviceResponse.StatusCode, serviceResponse);
        }

        // POST: api/rooms
        [Permission(CONSTANT.PERMISSION.ROOM.CREATE)]
        [HttpPost]
        public async Task<IActionResult> CreateRoom(
            [FromBody] CreateRoomRequest request)
        {
            var result = await _roomServices.Insert(request);
            return StatusCode(result.StatusCode, result);
        }

        // PUT: api/rooms/1
        [Permission(CONSTANT.PERMISSION.ROOM.UPDATE)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoom(
            int id,
            [FromBody] UpdateRoomRequest request)
        {
            var result = await _roomServices.Update(id, request);
            return StatusCode(result.StatusCode, result);
        }

        // DELETE: api/rooms/1
        [Permission(CONSTANT.PERMISSION.ROOM.DELETE)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(
            int id)
        {
            var result = await _roomServices.Delete(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}
