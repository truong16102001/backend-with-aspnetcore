using Microsoft.AspNetCore.Mvc;
using NetCore.DataAccess.DataObject.DTOs.Hotel;
using NetCore.DataAccess.IServices;

namespace NetCore.API.Controllers
{
    [Route("api/hotels")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly IHotelServices _hotelServices;

        public HotelController(
            IHotelServices hotelServices)
        {
            _hotelServices = hotelServices;
        }

        // GET api/hotels
        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] HotelRequest request)
        {
            var result = await _hotelServices.GetList(request);

            return StatusCode(result.StatusCode, result);
        }

        // GET api/hotels/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _hotelServices.GetById(id);

            return StatusCode(result.StatusCode, result);
        }

        // POST api/hotels
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateHotelRequest request)
        {
            var result = await _hotelServices.Insert(request);

            return StatusCode(result.StatusCode, result);
        }

        // PUT api/hotels/1
        [HttpPut("{id}")]
        public async Task<IActionResult>
            Update(int id, [FromBody] UpdateHotelRequest request)
        {
            var result = await _hotelServices.Update(id, request);

            return StatusCode(result.StatusCode, result);
        }

        // DELETE api/hotels/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _hotelServices.Delete(id);

            return StatusCode(result.StatusCode, result);
        }
    }
}