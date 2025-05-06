using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tea.Application.DTOs.Orders;
using Tea.Application.Services.Interfaces;
using Tea.Domain.Common;
using Tea.Domain.Constants;

namespace Tea.Api.Controllers
{
    [Authorize]
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

        [HttpGet]
        [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPagination([FromQuery]OrderPaginationRequest request)
        {
            var response = await orderService.GetPaginationAsync(request);
            return Ok(response);
        }

        [Authorize(Policy = AppPermission.Order_Create)]
        [HttpPost("in-store")]
        [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateInStore([FromBody] OrderCreateInStoreRequest request)
        {
            var response = await orderService.CreateInStoreAsync(request);
            return CreatedAtAction(nameof(Get), new { id = response.Id }, response);
        }

        [HttpPut("{id}/order-status")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateOrderStatus([FromRoute] int id, [FromBody] UpdateOrderStatusRequest request)
        {
            await orderService.UpdateOrderStatusAsync(id, request.Status);
            return NoContent();
        }

        [HttpPut("{id}/payment-status")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePaymentStatus([FromRoute] int id, [FromBody] UpdatePaymentStatusRequest request)
        {
            await orderService.UpdatePaymentStatusAsync(id, request.Status);
            return NoContent();
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
