using Tea.Domain.Common;
using Tea.Domain.Entities;

namespace Tea.Domain.Repositories
{
    public interface IDiscountRepository : IRepository<Discount>
    {
        Task<PaginationResponse<Discount>> GetPaginationAsync(PaginationRequest request);
    }
}
