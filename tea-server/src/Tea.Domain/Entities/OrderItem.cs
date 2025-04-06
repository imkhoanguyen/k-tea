namespace Tea.Domain.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }
        public required string ItemName { get; set; }
        public decimal Price { get; set; }
        public required string ItemSize { get; set; }
        public required string ItemImg { get; set; }
        public int Quantity { get; set; }
        public int ItemId { get; set; }
        public int OrderId { get; set; }
        //nav
        public Item? Item { get; set; }
        public Order? Order { get; set; }

        public decimal GetTotal() => Price * Quantity;
    }
}
