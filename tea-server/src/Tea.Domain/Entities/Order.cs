using System.ComponentModel.DataAnnotations.Schema;
using Tea.Domain.Enums;

namespace Tea.Domain.Entities
{
    public class Order
    {
        // order information
        public int Id { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public OrderType OrderType { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public PaymentType PaymentType { get; set; }
        public DateTimeOffset Created { get; set; } = DateTimeOffset.Now;
        public decimal SubTotal { get; set; } // sum price of items
        public decimal? DiscountPrice { get; set; }
        public decimal? ShippingFee { get; set; }
        public int? DiscountId { get; set; }
        public List<OrderItem> Items { get; set; } = [];
        public string? Description { get; set; }
        public bool IsDeleted { get; set; }

        // customer information
        public string? UserId { get; set; }
        public string? CustomerAddress { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }

        // employee information
        public string? CreatedById { get; set; }

        //nav
        public AppUser? AppUser { get; set; }
        public Discount? Discount { get; set; }
        public AppUser? CreatedBy { get; set; }

        public decimal GetTotal() => SubTotal + (ShippingFee ?? 0) - (DiscountPrice ?? 0);
    }
}
