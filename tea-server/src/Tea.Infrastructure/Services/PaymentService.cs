using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Tea.Application.DTOs.Orders;
using Tea.Application.DTOs.Payments;
using Tea.Application.Interfaces;
using Tea.Application.Mappers;
using Tea.Application.Services.Interfaces;
using Tea.Domain.Entities;
using Tea.Domain.Enums;
using Tea.Domain.Exceptions.BadRequests;
using Tea.Domain.Exceptions.NotFounds;
using Tea.Domain.Repositories;
using Tea.Infrastructure.Configurations;
using Tea.Infrastructure.Interfaces;
using Tea.Infrastructure.Utilities;

namespace Tea.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly VNPayConfig _config;
        private readonly IUnitOfWork _unit;
        private readonly ICartService _cartService;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<PaymentService> _logger;


        public PaymentService(IOptions<VNPayConfig> config, IUnitOfWork unit, 
            IOrderService orderService, ICartService cartService, 
            UserManager<AppUser> userManager, ILogger<PaymentService> logger)
        {
            var vnpayConfig = config.Value;
            _config = vnpayConfig;
            _unit = unit;
            _userManager = userManager;
            _logger = logger;
            _cartService = cartService;
        }
        public string CreatePaymentUrl(PaymentRequest request, HttpContext context)
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_config.TimeZoneId);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = DateTime.Now.Ticks.ToString();
            var pay = new VNPayLibary();
            var urlCallBack = _config.ReturnUrl;

            // Xử lý giảm giá
            decimal total = request.Total;
            if (request.DiscountPrice.HasValue && request.DiscountPrice.Value > 0 && request.DiscountPrice.Value <= request.Total)
            {
                total = request.Total - request.DiscountPrice.Value;
            }

            // Đóng gói thông tin OrderInfo
            var orderInfo = new
            {
                UserName = request.UserName,
                DiscountPrice = request.DiscountPrice,
                DiscountId = request.DiscountId,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                Description = request.Description,
            };

            pay.AddRequestData("vnp_Version", _config.Version);
            pay.AddRequestData("vnp_Command", _config.Command);
            pay.AddRequestData("vnp_TmnCode", _config.TmnCode);
            pay.AddRequestData("vnp_Amount", ((int)total * 100).ToString()); // VNPAY yêu cầu số tiền nhân 100
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _config.CurrCode);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", _config.Locale);
            pay.AddRequestData("vnp_OrderInfo", JsonConvert.SerializeObject(orderInfo));
            pay.AddRequestData("vnp_OrderType", "order");
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
            pay.AddRequestData("vnp_TxnRef", tick);

            return pay.CreateRequestUrl(_config.BaseUrl, _config.HashSecret);
        }

        public async Task<OrderResponse?> HandlePayment(PaymentReturnRequest request)
        {
            if (request.ResponseCode == "00")
            {
                var user = await _userManager.FindByNameAsync(request.UserName);
                if(user  == null)
                {
                    _logger.LogWarning($"username: {request.UserName} not found");
                    throw new UserNotFoundException(request.UserName);
                }

                var cart = await _cartService.GetCartAsync(request.UserName);

                if (cart == null && cart.Items.Count == 0)
                {
                    _logger.LogWarning($"cart with key(username): {request.UserName} not found Or cart items is empty");
                    throw new CartNotFoundException(request.UserName);
                }

                var listOrderItem = cart.Items.Select(x => new OrderItem
                {
                    ItemSize = x.Size,
                    ItemImg = x.ItemImg,
                    ItemName = x.ItemName,
                    Quantity = x.Quantity,
                    Price = x.Price,
                    ItemId = x.ItemId,
                }).ToList();

                var order = new Order
                {
                    UserId = user.Id,
                   
                    OrderStatus = OrderStatus.Pending,
                    OrderType = OrderType.Online,
                    PaymentStatus = PaymentStatus.Paid,
                    PaymentType = PaymentType.CreditCard,
                    Items = listOrderItem,
                    CustomerAddress = request.Address,
                    CustomerName = user.FullName,
                    CustomerPhone = request.PhoneNumber,
                    Description = request.Description,
                    SubTotal = listOrderItem.Sum(x => x.GetTotal())
                };

                if(request.DiscountId.HasValue && request.DiscountId > 0 && request.DiscountPrice.HasValue && request.DiscountPrice > 0)
                {
                    order.DiscountPrice = request.DiscountPrice;
                    order.DiscountId = request.DiscountId;
                }

                await _unit.BeginTransactionAsync();

                try
                {
                    _unit.Order.Add(order);
                    if(await _unit.SaveChangesAsync())
                    {
                        await _unit.CommitTransactionAsync();
                        _logger.LogInformation("create order with payment type: credit card succressfully");
                        return OrderMapper.EntityToResponse(order);
                    }

                    _logger.LogError("save change order with payment type: credit card failed");
                    throw new SaveChangesFailedException();
                }
                catch
                {
                    await _unit.RollbackTransactionAsync(); throw;
                }
            }

            return null;
        }
    }
}
