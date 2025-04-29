using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tea.Application.Interfaces;
using Tea.Domain.Entities;
using Tea.Domain.Exceptions.BadRequests;
using Tea.Domain.Exceptions.NotFounds;

namespace Tea.Api.Controllers
{
    [Authorize]
    public class CartsController(ICartService cartService) : BaseApiController
    {
        [HttpGet]
        [ProducesResponseType(typeof(ShoppingCart), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCartById(string id)
        {
            var cart = await cartService.GetCartAsync(id);
            return Ok(cart ?? new ShoppingCart { Id = id });
        }

        [HttpPost]
        [ProducesResponseType(typeof(ShoppingCart), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCart(ShoppingCart cart)
        {
            var updatedCart = await cartService.SetCartAsync(cart);
            if (updatedCart == null)
                throw new CartNotFoundException("Không tìm thấy giỏ hàng. Vui lòng thử lại sau");

            return Ok(updatedCart);
        }

        [HttpDelete]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteCart(string id)
        {
            var result = await cartService.DeleteCartAsync(id);
            if (!result)
                throw new DeleteCartFailedException("Có lỗi xảy ra khi xóa giỏ hàng.");

            return NoContent();
        }
    }
}
