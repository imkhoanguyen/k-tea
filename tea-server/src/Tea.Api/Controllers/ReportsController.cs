using Microsoft.AspNetCore.Mvc;
using Tea.Application.DTOs.Orders;
using Tea.Application.DTOs.Reports;
using Tea.Application.Services.Implements;
using Tea.Application.Services.Interfaces;
using Tea.Domain.Common;

namespace Tea.Api.Controllers
{
    public class ReportsController(IReportService reportService) : BaseApiController
    {
        [HttpGet]
        [ProducesResponseType(typeof(ReportResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            var totalOrder = await reportService.GetTotalOrdersPerDayAsync();
            var totalRevenue = await reportService.GetRevenuePerDayAsync();
            var totalUser = reportService.GetTotalUsers();

            var response = new ReportResponse
            {
                TotalOrderPerDay = totalOrder,
                TotalRevenuePerDay = totalRevenue,
                TotalUser = totalUser
            };

            return Ok(response);
        }

        [HttpGet("get-daily-revenue-in-month")]
        [ProducesResponseType(typeof(IEnumerable<DailyRevenueInMonthResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDailyRevenueInMonth([FromQuery]GetDailyRevenueInMonthRequest request)
        {
            var response = await reportService.GetDailyRevenueInMonthAsync(request.Month, request.Year);

            return Ok(response);
        }

        [HttpGet("get-top-selling-items")]
        [ProducesResponseType(typeof(IEnumerable<TopSellingItemResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTopSellingItems([FromQuery] int topCount)
        {
            var response = await reportService.GetTopSellingItemsAsync(topCount);

            return Ok(response);
        }
    }
}
