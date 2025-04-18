using Microsoft.AspNetCore.Identity;
using Tea.Application.Services.Interfaces;
using Tea.Domain.Common;
using Tea.Domain.Entities;
using Tea.Domain.Repositories;

namespace Tea.Application.Services.Implements
{
    public class ReportService(UserManager<AppUser> userManager, IUnitOfWork unit) : IReportService
    {
        public async Task<decimal> GetRevenuePerDayAsync()
        {
            return await unit.Order.GetRevenuePerDayAsync();
        }

        public async Task<IEnumerable<TopSellingItemResponse>> GetTopSellingItemsAsync(int topCount)
        {
            return await unit.Order.GetTopSellingItemsAsync(topCount);
        }

        public async Task<int> GetTotalOrdersPerDayAsync()
        {
            return await unit.Order.GetTotalOrdersPerToDayAsync();
        }

        public int GetTotalUsers()
        {
            var total = userManager.Users.ToList().Count();
            return total;
        }

        public async Task<IEnumerable<DailyRevenueInMonthResponse>> GetDailyRevenueInMonthAsync(int month, int? year)
        {
            var response = await unit.Order.GetDailyRevenueInMonthAsync(month, year ?? DateTime.Now.Year);
            return response;
        }
    }
}
