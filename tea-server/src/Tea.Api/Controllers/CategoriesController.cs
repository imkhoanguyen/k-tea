using Microsoft.AspNetCore.Mvc;
using Tea.Application.DTOs.Categories;
using Tea.Application.Services.Interfaces;
using Tea.Domain.Common;

namespace Tea.Api.Controllers
{
    public class CategoriesController(ICategoryService categoryService) : BaseApiController
    {
        [HttpGet]
        [ProducesResponseType(typeof(PaginationResponse<CategoryResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPagination([FromQuery] PaginationRequest request)
        {
            var pagination = await categoryService.GetPaginationAsync(request);
            return Ok(pagination);
        }

        [HttpGet("all")]
        [ProducesResponseType(typeof(IEnumerable<CategoryResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var all = await categoryService.GetAllAsync();
            return Ok(all);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            var categoryResponse = await categoryService.GetByIdAsync(id);
            return Ok(categoryResponse);
        }

        [HttpPost]
        [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] CategoryCreateParentRequest request)
        {
            var categoryResponse = await categoryService.CreateParentAsync(request);
            return CreatedAtAction(nameof(Get), new { id = categoryResponse.Id }, categoryResponse);
        }

        [HttpPost("children")]
        [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateChildren([FromBody] CategoryCreateChildrenRequest request)
        {
            var categoryResponse = await categoryService.CreateChildrenAsync(request);
            return CreatedAtAction(nameof(Get), new { id = categoryResponse.Id }, categoryResponse);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] CategoryUpdateRequest request)
        {
            var categoryResponse = await categoryService.UpdateAsync(id, request);
            return Ok(categoryResponse);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await categoryService.DeleteAsync(id);
            return NoContent();
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Deletes([FromQuery] List<int> categoryIdList)
        {
            await categoryService.DeletesAsync(categoryIdList);
            return NoContent();
        }
    }
}
