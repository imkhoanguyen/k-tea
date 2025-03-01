using Tea.Application.DTOs.Items;
using Tea.Domain.Common;
using Tea.Domain.Entities;

namespace Tea.Application.Services.Interfaces
{
    public interface IItemService
    {
        Task<PaginationResponse<ItemResponse>> GetPaginationAsync(PaginationRequest request);
        Task<ItemResponse> GetByIdAsync(int id);
        Task<ItemResponse> CreateAsync(ItemCreateRequest request);
        //Task<ItemResponse> UpdateAsync(int id, CategoryUpdateRequest request);
        //Task DeleteAsync(int id);
    }
}
