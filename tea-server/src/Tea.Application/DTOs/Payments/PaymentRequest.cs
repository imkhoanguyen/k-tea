namespace Tea.Application.DTOs.Payments
{
    public class PaymentRequest
    {
        public decimal Total { get; set; }
        public required string UserName { get; set; }
        public int? DiscountId { get; set; }
        public decimal? DiscountPrice { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Address { get; set; }
        public string? Description { get; set; }
    }
}
