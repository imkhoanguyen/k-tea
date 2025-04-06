using System.Linq.Expressions;
using Tea.Domain.Common;
using Tea.Domain.Entities;
using Tea.Domain.Repositories;
using Tea.Infrastructure.DataAccess;

namespace Tea.Infrastructure.Repositories
{
    public class OrderRepository(TeaContext context) : Repository<Order>(context), IOrderRepository
    {
        public Task<PaginationResponse<Order>> GetPaginationAsync(PaginationRequest request, Expression<Func<Order, bool>>? expression)
        {
            throw new NotImplementedException();
        }
    }
}
