using System.Linq.Expressions;
using Tea.Domain.Common;
using Tea.Domain.Entities;

namespace Tea.Domain.Repositories
{
    public interface IItemRepository : IRepository<Item>
    {
        Task<PaginationResponse<Item>> GetPaginationAsync(ItemPaginationRequest request, Expression<Func<Item, bool>>? expression);
    }
}
