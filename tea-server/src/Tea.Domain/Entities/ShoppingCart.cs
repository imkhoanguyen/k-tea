using System.ComponentModel.DataAnnotations.Schema;

namespace Tea.Domain.Entities
{
    [NotMapped]
    public class ShoppingCart
    {
        public required string Id { get; set; } // username
        public List<CartItem> Items { get; set; } = [];
    }
}
