using Tea.Application.DTOs.Categories;
using Tea.Domain.Common;

namespace Tea.Application.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<PaginationResponse<CategoryResponse>> GetPaginationAsync(PaginationRequest request);
        Task<CategoryResponse> GetByIdAsync(int id);
        Task<IEnumerable<CategoryResponse>> GetAllAsync();
        Task<CategoryResponse> CreateParentAsync(CategoryCreateParentRequest request);
        Task<CategoryResponse> CreateChildrenAsync(CategoryCreateChildrenRequest request);
        Task<CategoryResponse> UpdateAsync(int id, CategoryUpdateRequest request);
        Task DeleteAsync(int id);
        Task DeletesAsync(List<int> categoryIdList);
    }
}
