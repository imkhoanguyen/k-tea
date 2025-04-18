using Tea.Application.DTOs.Orders;
using Tea.Domain.Common;
using Tea.Domain.Entities;

namespace Tea.Domain.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<PaginationResponse<OrderListResponse>> GetPaginationAsync(OrderPaginationRequest request);
        Task<int> GetTotalOrdersPerToDayAsync();
        Task<decimal> GetRevenuePerDayAsync();
        Task<IEnumerable<TopSellingItemResponse>> GetTopSellingItemsAsync(int topCount);
        Task<IEnumerable<DailyRevenueInMonthResponse>> GetDailyRevenueInMonthAsync(int monthNumber, int year);

    }
}
