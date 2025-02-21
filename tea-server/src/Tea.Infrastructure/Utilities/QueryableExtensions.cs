using Microsoft.EntityFrameworkCore;
using Tea.Domain.Common;

namespace Tea.Infrastructure.Utilities
{
    public static class QueryableExtensions
    {
        public static async Task<PaginationResponse<T>> ApplyPaginationAsync<T>(this IQueryable<T> query, int page, int pageSize) where T : class
        {
            var totalItems = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginationResponse<T>(items, totalItems, page, pageSize);
        }
    }
}
