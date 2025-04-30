namespace Tea.Application.DTOs.Orders
{
    public class OrderListResponse
    {
        public int Id { get; set; }
        public string? OrderStatus { get; set; }
        public string? OrderType { get; set; }
        public string PaymentStatus { get; set; }
        public string? PaymentType { get; set; }
        public DateTimeOffset Created { get; set; } 
        public decimal SubTotal { get; set; } // sum price of items
        public decimal? DiscountPrice { get; set; }
        public decimal? ShippingFee { get; set; }
        public int? DiscountId { get; set; }
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
