using Microsoft.AspNetCore.Http;
using Tea.Application.DTOs.Items;
using Tea.Application.DTOs.Sizes;
using Tea.Domain.Common;

namespace Tea.Application.Services.Interfaces
{
    public interface IItemService
    {
        Task<PaginationResponse<ItemResponse>> GetPaginationAsync(ItemPaginationRequest request);
        Task<PaginationResponse<ItemResponse>> GetPublicPaginationAsync(ItemPaginationRequest request);
        Task<ItemResponse> GetByIdAsync(int id);
        Task<ItemResponse> CreateAsync(ItemCreateRequest request);
        // update item info and cateogry of item
        Task<ItemResponse> UpdateItemAsync(int id, ItemUpdateRequest request);
        // update size info of item
        Task<ItemResponse> UpdateSizeAsync(int itemId, SizeUpdateRequest request);
        // update img url of item
        Task<string> UpdateImageAsync(int itemId, IFormFile imgFile);
        Task<ItemResponse> AddSizesAsync(int itemId, List<SizeCreateRequest> requests);
        Task DeleteSizesAsync(int itemId, List<int> sizeIdList);
        Task DeleteAsync(int id);
        Task DeletesAsync(List<int> itemIdList);
    }
}
