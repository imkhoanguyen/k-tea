using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tea.Application.DTOs.Items;
using Tea.Application.DTOs.Orders;
using Tea.Application.DTOs.Sizes;
using Tea.Application.Services.Implements;
using Tea.Application.Services.Interfaces;
using Tea.Domain.Common;
using Tea.Domain.Constants;
using Tea.Domain.Exceptions;
using Tea.Infrastructure.Interfaces;
using Tea.Infrastructure.Services;

namespace Tea.Api.Controllers
{
    [Authorize]
    public class ItemsController(IItemService itemService, IExcelService excelService) : BaseApiController
    {
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(typeof(PaginationResponse<ItemResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPagination([FromQuery] ItemPaginationRequest request)
        {
            var pagination = await itemService.GetPaginationAsync(request);
            return Ok(pagination);
        }

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ItemResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            var itemResponse = await itemService.GetByIdAsync(id);
            return Ok(itemResponse);
        }

        [Authorize(Policy = AppPermission.Item_Create)]
        [HttpPost]
        [ProducesResponseType(typeof(ItemResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromForm] ItemCreateRequest request)
        {
            var itemResponse = await itemService.CreateAsync(request);
            return CreatedAtAction(nameof(Get), new { id = itemResponse.Id }, itemResponse);
        }

        [Authorize(Policy = AppPermission.Item_Create)]
        [HttpPost("{itemId:int}/sizes")]
        [ProducesResponseType(typeof(ItemResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddSizesToItem([FromRoute] int itemId, [FromBody] List<SizeCreateRequest> requests)
        {
            var itemResponse = await itemService.AddSizesAsync(itemId, requests);
            return CreatedAtAction(nameof(Get), new { id = itemResponse.Id }, itemResponse);
        }

        [Authorize(Policy = AppPermission.Item_Edit)]
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ItemResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateItem([FromRoute] int id, [FromBody] ItemUpdateRequest request)
        {
            var itemResponse = await itemService.UpdateItemAsync(id, request);
            return Ok(itemResponse);
        }

        [Authorize(Policy = AppPermission.Item_Edit)]
        [HttpPut("{id:int}/image")]
        [ProducesResponseType(typeof(ItemResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateImageOfItem([FromRoute] int id, IFormFile imgFile)
        {
            var itemResponse = await itemService.UpdateImageAsync(id, imgFile);
            return Ok(new { imgUrl = itemResponse });
        }

        [Authorize(Policy = AppPermission.Item_Edit)]
        [HttpPut("{id:int}/size")]
        [ProducesResponseType(typeof(ItemResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateSizeOfItem([FromRoute] int id, [FromBody] SizeUpdateRequest request)
        {
            var itemResponse = await itemService.UpdateSizeAsync(id, request);
            return Ok(itemResponse);
        }

        [Authorize(Policy = AppPermission.Item_Delete)]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ItemResponse), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await itemService.DeleteAsync(id);
            return NoContent();
        }

        [Authorize(Policy = AppPermission.Item_Delete)]
        [HttpDelete]
        [ProducesResponseType(typeof(ItemResponse), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Deletes([FromQuery] List<int> itemIdList)
        {
            await itemService.DeletesAsync(itemIdList);
            return NoContent();
        }

        [Authorize(Policy = AppPermission.Item_Delete)]
        [HttpDelete("{itemId:int}/sizes")]
        [ProducesResponseType(typeof(ItemResponse), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteSizesOfItem([FromRoute] int itemId, [FromQuery] List<int> sizeIdList)
        {
            await itemService.DeleteSizesAsync(itemId, sizeIdList);
            return NoContent();
        }

        [HttpPost("export-template-update")]
        public async Task<IActionResult> ExportTemplateAddItem([FromBody] List<int> ids)
        {
            var stream = await excelService.ExportTemplateUpdateItemAsync(ids);

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"UpdateItemExample{DateTime.Now.Ticks}.xlsx");
        }

        [HttpPost("import-update-items")]
        public async Task<IActionResult> ImportUpdateItemsFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Only .xlsx files are allowed");
            }


            using var stream = file.OpenReadStream();
            var result = await excelService.ImportUpdateItemsFromExcelAsync(stream);
            return Ok(result);

        }
    }
}
