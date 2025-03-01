using Microsoft.AspNetCore.Mvc;
using Tea.Application.DTOs.Items;
using Tea.Application.Services.Interfaces;

namespace Tea.Api.Controllers
{
    public class ItemsController(IItemService itemService) : BaseApiController
    {
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ItemResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            var itemResponse = await itemService.GetByIdAsync(id);
            return Ok(itemResponse);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ItemResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromForm] ItemCreateRequest request)
        {
            var itemResponse = await itemService.CreateAsync(request);
            return CreatedAtAction(nameof(Get), new { id = itemResponse.Id }, itemResponse);
        }
    }
}
