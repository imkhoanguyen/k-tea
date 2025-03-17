using Microsoft.AspNetCore.Mvc;
using Tea.Application.DTOs.Items;
using Tea.Application.DTOs.Sizes;
using Tea.Application.Services.Interfaces;
using Tea.Domain.Common;

namespace Tea.Api.Controllers
{
    public class ItemsController(IItemService itemService) : BaseApiController
    {
        [HttpGet]
        [ProducesResponseType(typeof(PaginationResponse<ItemResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPagination([FromQuery] PaginationRequest request)
        {
            var pagination = await itemService.GetPaginationAsync(request);
            return Ok(pagination);
        }

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

        [HttpPost("{itemId:int}/sizes")]
        [ProducesResponseType(typeof(ItemResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddSizesToItem([FromRoute] int itemId, [FromBody] List<SizeCreateRequest> requests)
        {
            var itemResponse = await itemService.AddSizesAsync(itemId, requests);
            return CreatedAtAction(nameof(Get), new { id = itemResponse.Id }, itemResponse);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ItemResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateItem([FromRoute] int id, [FromBody] ItemUpdateRequest request)
        {
            var itemResponse = await itemService.UpdateItemAsync(id, request);
            return Ok(itemResponse);
        }

        [HttpPut("{id:int}/image")]
        [ProducesResponseType(typeof(ItemResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateImageOfItem([FromRoute] int id, IFormFile imgFile)
        {
            var itemResponse = await itemService.UpdateImageAsync(id, imgFile);
            return Ok(new {imgUrl = itemResponse});
        }

        [HttpPut("{id:int}/size")]
        [ProducesResponseType(typeof(ItemResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateSizeOfItem([FromRoute] int id, [FromBody] SizeUpdateRequest request)
        {
            var itemResponse = await itemService.UpdateSizeAsync(id, request);
            return Ok(itemResponse);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ItemResponse), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await itemService.DeleteAsync(id);
            return NoContent();
        }

        [HttpDelete("{itemId:int}/sizes")]
        [ProducesResponseType(typeof(ItemResponse), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteSizesOfItem([FromRoute] int itemId,[FromQuery] List<int> sizeIdList)
        {
            await itemService.DeleteSizesAsync(itemId, sizeIdList);
            return NoContent();
        }
    }
}
