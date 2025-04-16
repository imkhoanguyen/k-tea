using Tea.Domain.Enums;

namespace Tea.Application.DTOs.Orders
{
    public class OrderCreateOnlineRequest
    {
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
        public OrderType OrderType { get; set; } = OrderType.Online;
        public PaymentStatus PaymentStatus { get; set; }
        public required string PaymentType { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int? DiscountId { get; set; }
        public List<OrderItemCreateRequest> Items { get; set; } = [];
        public string? UserName { get; set; }
        public string? CustomerAddress { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? Description { get; set; }
    }
}
