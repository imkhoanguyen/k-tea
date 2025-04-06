using System.ComponentModel.DataAnnotations.Schema;

namespace Tea.Domain.Entities
{
    [NotMapped]
    public class CartItem
    {
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public required string ItemName { get; set; }
        public required string Size { get; set; }
        public required string ItemImg { get; set; }
        public int ItemId { get; set; }

        public decimal GetTotal() => Price * Quantity;
    }
}
