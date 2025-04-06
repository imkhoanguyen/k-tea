using Tea.Domain.Enums;

namespace Tea.Application.DTOs.Orders
{
    public class OrderCreateInStoreRequest
    {
        public required string CreatedById { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int? DiscountId { get; set; }
        public List<OrderItemCreateRequest> Items { get; set; } = [];
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Completed;
        public OrderType OrderType { get; set; } = OrderType.InStore;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Paid;
        public required string PaymentType { get; set; }
    }
}
