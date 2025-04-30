using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tea.Api.Extensions;
using Tea.Application.DTOs.Orders;
using Tea.Application.DTOs.Reports;
using Tea.Application.Services.Interfaces;
using Tea.Domain.Common;
using Tea.Infrastructure.Interfaces;

namespace Tea.Api.Controllers
{
    [Authorize]
    public class ReportsController(IReportService reportService, IPdfService pdfService, 
        IOrderService orderService, IExcelService excelService) : BaseApiController
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
        public async Task<IActionResult> GetDailyRevenueInMonth([FromQuery] GetDailyRevenueInMonthRequest request)
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

        [HttpGet("export")]
        public async Task<IActionResult> ExportExcelOrders([FromQuery] OrderPaginationRequest request)
        {
            request.PageSize = int.MaxValue;

            var paginationResponse = await orderService.GetPaginationAsync(request);

            var stream = await excelService.Export<OrderListResponse>(paginationResponse.Data.ToList());

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"OrderReport{DateTime.Now.Ticks}.xlsx");
        }


        [HttpGet("print")]
        public async Task<IActionResult> ExportPdfOrder([FromQuery]int orderId)
        {
            var order = await orderService.GetByIdAsync(orderId);
            var html = $@"
    <html>
    <head>
        <style>
            body {{ font-family: Arial, sans-serif; margin: 20px; }}
            .header {{ text-align: center; margin-bottom: 30px; }}
            .header h1 {{ color: #0066cc; margin-bottom: 5px; }}
            .order-info {{ margin-bottom: 20px; }}
            .section {{ margin-bottom: 15px; }}
            .section-title {{ font-weight: bold; margin-bottom: 5px; border-bottom: 1px solid #ddd; }}
            .customer-info, .employee-info, .payment-info {{ 
                display: flex; 
                flex-wrap: wrap; 
                justify-content: space-between;
                margin-bottom: 15px;
            }}
            .info-block {{ width: 48%; margin-bottom: 10px; }}
            .items-table {{ 
                width: 100%; 
                border-collapse: collapse;
                margin-bottom: 20px;
            }}
            .items-table th {{ 
                background-color: #f2f2f2;
                text-align: left;
                padding: 8px;
                border: 1px solid #ddd;
            }}
            .items-table td {{
                padding: 8px;
                border: 1px solid #ddd;
            }}
            .totals {{ 
                margin-left: auto; 
                width: 300px;
                border: 1px solid #ddd;
                padding: 10px;
            }}
            .totals-row {{ display: flex; justify-content: space-between; }}
            .total-row {{ font-weight: bold; border-top: 1px solid #ddd; margin-top: 5px; padding-top: 5px; }}
            .footer {{ margin-top: 30px; font-size: 0.8em; text-align: center; color: #666; }}
        </style>
    </head>
    <body>
        <div class='header'>
            <h1>K TEA</h1>
            <p>Order #: {order.Id}</p>
            <p>Date: {order.Created:yyyy-MM-dd HH:mm}</p>
        </div>

        <div class='customer-info'>
            <div class='info-block'>
                <div class='section-title'>CUSTOMER INFORMATION</div>
                <p><strong>Name:</strong> {order.CustomerName}</p>
                <p><strong>Phone:</strong> {order.CustomerPhone}</p>
                <p><strong>Address:</strong> {order.CustomerAddress}</p>
            </div>
            
            <div class='info-block'>
                <div class='section-title'>ORDER DETAILS</div>
                <p><strong>Status:</strong> {order.OrderStatus}</p>
                <p><strong>Type:</strong> {order.OrderType}</p>
                <p><strong>Description:</strong> {order.Description ?? "N/A"}</p>
            </div>
        </div>

        <div class='section'>
            <div class='section-title'>ORDER ITEMS</div>
            <table class='items-table'>
                <thead>
                    <tr>
                        <th>Item</th>
                        <th>Quantity</th>
                        <th>Unit Price</th>
                        <th>Total</th>
                    </tr>
                </thead>
                <tbody>
                    {GenerateOrderItemsRows(order.Items)}
                </tbody>
            </table>
        </div>

        <div class='totals'>
            <div class='totals-row'>
                <span>Subtotal:</span>
                <span>{order.SubTotal.ToVndCurrency()}</span>
            </div>
            <div class='totals-row'>
                <span>Shipping Fee:</span>
                <span>{(order.ShippingFee ?? 0).ToVndCurrency()}</span>
            </div>
            <div class='totals-row'>
                <span>Discount:</span>
                <span>-{(order.DiscountPrice ?? 0).ToVndCurrency()}</span>
            </div>
            <div class='totals-row total-row'>
                <span>TOTAL:</span>
                <span>{order.Total.ToVndCurrency()}</span>
            </div>
        </div>

        <div class='payment-info'>
            <div class='info-block'>
                <div class='section-title'>PAYMENT INFORMATION</div>
                <p><strong>Status:</strong> {order.PaymentStatus}</p>
                <p><strong>Method:</strong> {order.PaymentType}</p>
            </div>
            
            <div class='info-block'>
                <div class='section-title'>PROCESSED BY</div>
                <p><strong>Employee ID:</strong> {order.CreatedById ?? "N/A"}</p>
            </div>
        </div>

        <div class='footer'>
            <p>Thank you for your business!</p>
            <p>Generated on {DateTime.Now:yyyy-MM-dd HH:mm}</p>
        </div>
    </body>
    </html>";

            var result = pdfService.GeneratePdf(html);
            return File(result, "application/pdf", $"Order_{order.Id}_{DateTime.Now:yyyyMMddHHmmss}.pdf");
        }

        private string GenerateOrderItemsRows(List<OrderItemResponse> items)
        {
            if (items == null || items.Count == 0)
            {
                return "<tr><td colspan='4'>No items in this order</td></tr>";
            }

            var rows = new StringBuilder();
            foreach (var item in items)
            {
                rows.AppendLine($@"
            <tr>
                <td>{item.ItemName}</td>
                <td>{item.Quantity}</td>
                <td>{item.Price.ToVndCurrency()}</td>
                <td>{(item.Quantity * item.Price).ToVndCurrency()}</td>
            </tr>");
            }
            return rows.ToString();
        }
    }
}
