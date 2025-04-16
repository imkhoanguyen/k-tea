using Tea.Domain.Enums;

namespace Tea.Application.DTOs.Orders
{
    public class OrderResponse
    {
        public int Id { get; set; }
        public required string OrderStatus { get; set; }
        public required string OrderType { get; set; }
        public required string PaymentStatus { get; set; }
        public required string PaymentType { get; set; }
        public DateTimeOffset Created { get; set; } = DateTimeOffset.Now;
        public decimal SubTotal { get; set; } // sum price of items
        public decimal? DiscountPrice { get; set; }
        public decimal? ShippingFee { get; set; }
        public int? DiscountId { get; set; }
        public List<OrderItemResponse> Items { get; set; } = [];
        public string? Description { get; set; }

        // customer information
        public string? UserId { get; set; }
        public string? CustomerAddress { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }

        // employee information
        public string? CreatedById { get; set; }
        public decimal Total => SubTotal + (ShippingFee ?? 0) - (DiscountPrice ?? 0);
    }
}
