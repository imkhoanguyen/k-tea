using System.Linq.Expressions;
using Tea.Domain.Common;
using Tea.Domain.Entities;

namespace Tea.Domain.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<PaginationResponse<Order>> GetPaginationAsync(PaginationRequest request, Expression<Func<Order, bool>>? expression);
    }
}
