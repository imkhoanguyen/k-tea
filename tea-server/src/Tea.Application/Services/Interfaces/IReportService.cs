using Tea.Domain.Common;

namespace Tea.Application.Services.Interfaces
{
    public interface IReportService
    {
        int GetTotalUsers();
        Task<int> GetTotalOrdersPerDayAsync();
        Task<decimal> GetRevenuePerDayAsync();
        Task<IEnumerable<TopSellingItemResponse>> GetTopSellingItemsAsync(int topCount);
        Task<IEnumerable<DailyRevenueInMonthResponse>> GetDailyRevenueInMonthAsync(int month, int? year);

    }
}
