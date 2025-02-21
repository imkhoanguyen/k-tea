using Tea.Domain.Common;
using Tea.Domain.Entities;

namespace Tea.Domain.Repositories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<PaginationResponse<Category>> GetPaginationAsync(PaginationRequest request);
    }
}
