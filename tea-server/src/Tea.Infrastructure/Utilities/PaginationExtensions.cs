using Microsoft.EntityFrameworkCore;
using Tea.Domain.Common;

namespace Tea.Infrastructure.Utilities
{
    public static class PaginationExtensions
    {
        public static async Task<PaginationResponse<T>> ApplyPaginationAsync<T>(this IQueryable<T> query, int pageIndex, int pageSize) where T : class
        {
            var totalItems = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginationResponse<T>(pageIndex, pageSize, totalItems, items);
        }

        public static PaginationResponse<T> ApplyPagination<T>(this List<T> list, int pageIndex, int pageSize) where T : class
        {
            var totalItems = list.Count;
            var items = list.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            return new PaginationResponse<T>(pageIndex, pageSize, totalItems, items);
        }
    }
}
