namespace Tea.Application.DTOs.Orders
{
    public class OrderItemResponse
    {
        public int Id { get; set; }
        public required string ItemName { get; set; }
        public decimal Price { get; set; }
        public required string ItemSize { get; set; }
        public required string ItemImg { get; set; }
        public int Quantity { get; set; }
        public int ItemId { get; set; }
        public int OrderId { get; set; }
        public decimal Total => Price * Quantity;
    }
}
