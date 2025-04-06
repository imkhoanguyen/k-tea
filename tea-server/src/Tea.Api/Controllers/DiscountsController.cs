using Microsoft.AspNetCore.Mvc;
using Tea.Application.DTOs.Categories;
using Tea.Application.DTOs.Discounts;
using Tea.Application.Services.Implements;
using Tea.Application.Services.Interfaces;

namespace Tea.Api.Controllers
{
    public class DiscountsController(IDiscountService discountService) : BaseApiController
    {
        [HttpGet("{code}")]
        [ProducesResponseType(typeof(DiscountResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(string code)
        {
            var response = await discountService.GetByCodeAsync(code);
            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(typeof(DiscountResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] DiscountCreateRequest request)
        {
            var response = await discountService.CreateAsync(request);
            return CreatedAtAction(nameof(Get), new { code = response.Id }, response);
        }
    }
}
