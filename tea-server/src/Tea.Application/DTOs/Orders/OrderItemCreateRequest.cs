namespace Tea.Application.DTOs.Orders
{
    public class OrderItemCreateRequest
    {
        public required string ItemName { get; set; }
        public decimal Price { get; set; }
        public required string ItemSize { get; set; }
        public required string ItemImg { get; set; }
        public int Quantity { get; set; }
        public int ItemId { get; set; }
    }
}
