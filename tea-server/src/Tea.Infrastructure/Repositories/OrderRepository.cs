using System.Data;
using System.Linq.Expressions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Tea.Application.DTOs.Orders;
using Tea.Domain.Common;
using Tea.Domain.Entities;
using Tea.Domain.Repositories;
using Tea.Infrastructure.DataAccess;

namespace Tea.Infrastructure.Repositories
{
    public class OrderRepository(TeaContext context) : Repository<Order>(context), IOrderRepository
    {
        public override async Task<Order?> FindAsync(Expression<Func<Order, bool>>? predicate, bool tracked = false)
        {
            if (predicate == null)
                return tracked ? await context.Orders.Include(x => x.Items).FirstOrDefaultAsync()
                : await context.Orders.Include(x => x.Items).AsNoTracking().FirstOrDefaultAsync();

            return tracked ? await context.Orders.Include(x => x.Items).FirstOrDefaultAsync(predicate)
                : await context.Orders.Include(x => x.Items).AsNoTracking().FirstOrDefaultAsync(predicate);
        }

        public async Task<PaginationResponse<OrderListResponse>> GetPaginationAsync(OrderPaginationRequest request)
        {
            var totalCountParam = new SqlParameter("@TotalCount", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            var parameters = new[]
            {
                new SqlParameter("@PageIndex", request.PageIndex),
                new SqlParameter("@PageSize", request.PageSize),
                new SqlParameter("@FromDate", request.FromDate ?? (object)DBNull.Value),
                new SqlParameter("@ToDate", request.ToDate ?? (object)DBNull.Value),
                new SqlParameter("@MinAmount", request.MinAmount ?? (object)DBNull.Value),
                new SqlParameter("@MaxAmount", request.MaxAmount ?? (object)DBNull.Value),
                new SqlParameter("@UserName", request.UserName ?? (object)DBNull.Value),
                new SqlParameter("@SearchKey", request.Search.IsNullOrEmpty() ? (object)DBNull.Value : request.Search),
                totalCountParam
            };

            var orders = await context.OrderListResponses
                .FromSqlRaw("EXEC [dbo].[sp_OrderPagination] @PageIndex, @PageSize, @FromDate, @ToDate, @MinAmount, @MaxAmount, @UserName, @SearchKey, @TotalCount OUTPUT", parameters)
                .ToListAsync();

            var totalCount = totalCountParam.Value != DBNull.Value ? (int)totalCountParam.Value : 0;

            return new PaginationResponse<OrderListResponse>(
                request.PageIndex,
                request.PageSize,
                totalCount,
                orders
            );
        }

        public async Task<int> GetTotalOrdersPerToDayAsync()
        {
            var today = DateTime.Today; // Lấy 00:00:00 của hôm nay
            var tomorrow = today.AddDays(1); // Lấy 00:00:00 của ngày mai

            return await context.Orders
            .Where(o => o.Created >= today && o.Created < tomorrow && !o.IsDeleted)
            .CountAsync();
        }

        public async Task<decimal> GetRevenuePerDayAsync()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            return await context.Orders
                .Where(o => o.Created >= today && o.Created < tomorrow && !o.IsDeleted)
                .SumAsync(o => o.SubTotal + (o.ShippingFee ?? 0) - (o.DiscountPrice ?? 0));
        }

        public async Task<IEnumerable<TopSellingItemResponse>> GetTopSellingItemsAsync(int topCount)
        {
            var parameter = new SqlParameter("@TopCount", topCount);

            var result = await context.TopSellingItemResponses
                .FromSqlRaw("EXEC [dbo].[sp_GetTopSellingItems] @TopCount", parameter)
                .ToListAsync();

            return result;
        }

        public async Task<IEnumerable<DailyRevenueInMonthResponse>> GetDailyRevenueInMonthAsync(int monthNumber, int year)
        {
            var parameters = new[]
            {
                new SqlParameter("@MonthNumber", monthNumber),
                new SqlParameter("@Year", year)
            };

            return await context.DailyRevenueInMonthResponses
                .FromSqlRaw("EXEC [dbo].[sp_GetDailyRevenueInMonth] @MonthNumber, @Year", parameters)
                .ToListAsync();
        }
    }
}
