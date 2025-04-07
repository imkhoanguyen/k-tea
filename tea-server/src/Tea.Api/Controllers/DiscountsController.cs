using Microsoft.AspNetCore.Mvc;
using Tea.Application.DTOs.Discounts;
using Tea.Application.Services.Interfaces;
using Tea.Domain.Common;

namespace Tea.Api.Controllers
{
    public class DiscountsController(IDiscountService discountService) : BaseApiController
    {
        [HttpGet]
        [ProducesResponseType(typeof(PaginationResponse<DiscountResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPagination([FromQuery] PaginationRequest request)
        {
            var pagination = await discountService.GetPaginationAsync(request);
            return Ok(pagination);
        }

        [HttpGet("all")]
        [ProducesResponseType(typeof(IEnumerable<DiscountResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var all = await discountService.GetAllAsync();
            return Ok(all);
        }

        [HttpGet("{code}")]
        [ProducesResponseType(typeof(DiscountResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CheckDiscount(string code)
        {
            var response = await discountService.CheckDiscountAsync(code);
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(DiscountResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            var response = await discountService.GetByIdAsync(id);
            return Ok(response);
        }


        [HttpPost]
        [ProducesResponseType(typeof(DiscountResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] DiscountCreateRequest request)
        {
            var response = await discountService.CreateAsync(request);
            return CreatedAtAction(nameof(Get), new { id = response.Id }, response);
        }


        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(DiscountResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] DiscountUpdateRequest request)
        {
            var DiscountResponse = await discountService.UpdateAsync(id, request);
            return Ok(DiscountResponse);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await discountService.DeleteAsync(id);
            return NoContent();
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Deletes([FromQuery] List<int> discountIdList)
        {
            await discountService.DeletesAsync(discountIdList);
            return NoContent();
        }
    }
}
