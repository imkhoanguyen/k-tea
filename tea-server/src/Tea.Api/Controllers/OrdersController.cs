using Microsoft.AspNetCore.Mvc;
using Tea.Application.DTOs.Orders;
using Tea.Application.Services.Interfaces;

namespace Tea.Api.Controllers
{
    public class OrdersController(IOrderService orderService) : BaseApiController
    {
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            var response = await orderService.GetByIdAsync(id);
            return Ok(response);
        }

        [HttpPost("in-store")]
        [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateInStore([FromBody] OrderCreateInStoreRequest request)
        {
            var response = await orderService.CreateInStoreAsync(request);
            return CreatedAtAction(nameof(Get), new { id = response.Id }, response);
        }

        [HttpPost("online")]
        [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateOnline([FromBody] OrderCreateOnlineRequest request)
        {
            var response = await orderService.CreateOnlineAsync(request);
            return CreatedAtAction(nameof(Get), new { id = response.Id }, response);
        }
    }
}
