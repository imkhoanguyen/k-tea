using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
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

        public override async Task<Order?> FindAsync(Expression<Func<Order, bool>>? predicate, bool tracked = false)
        {
            if (predicate == null)
                return tracked ? await context.Orders.Include(x => x.Items).FirstOrDefaultAsync()
                : await context.Orders.Include(x => x.Items).AsNoTracking().FirstOrDefaultAsync();

            return tracked ? await context.Orders.Include(x => x.Items).FirstOrDefaultAsync(predicate)
                : await context.Orders.Include(x => x.Items).AsNoTracking().FirstOrDefaultAsync(predicate);
        }
    }
}
