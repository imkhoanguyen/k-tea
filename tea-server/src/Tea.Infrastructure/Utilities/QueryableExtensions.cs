using Microsoft.EntityFrameworkCore;
using Tea.Domain.Common;

namespace Tea.Infrastructure.Utilities
{
    public static class QueryableExtensions
    {
        public static async Task<PaginationResponse<T>> ApplyPaginationAsync<T>(this IQueryable<T> query, int pageIndex, int pageSize) where T : class
        {
            var totalItems = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginationResponse<T>(pageIndex, pageSize, totalItems, items);
        }
    }
}
