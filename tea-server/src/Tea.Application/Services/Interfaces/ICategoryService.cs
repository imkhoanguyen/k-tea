using Tea.Application.DTOs.Categories;
using Tea.Domain.Common;

namespace Tea.Application.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<PaginationResponse<CategoryResponse>> GetPaginationAsync(PaginationRequest request);
        Task<CategoryResponse> GetByIdAsync(int id);
        Task<CategoryResponse> CreateAsync(CategoryCreateRequest request);
        Task<CategoryResponse> UpdateAsync(int id, CategoryUpdateRequest request);
        Task DeleteAsync(int id);
    }
}
