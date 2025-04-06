using Tea.Domain.Common;
using Tea.Domain.Entities;
using Tea.Domain.Repositories;
using Tea.Infrastructure.DataAccess;

namespace Tea.Infrastructure.Repositories
{
    public class DiscountRepository(TeaContext context) : Repository<Discount>(context), IDiscountRepository
    {
        public Task<PaginationResponse<Discount>> GetPaginationAsync(PaginationRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
