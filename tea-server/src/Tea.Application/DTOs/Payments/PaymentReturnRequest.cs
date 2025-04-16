using Tea.Application.DTOs.Orders;

namespace Tea.Application.DTOs.Payments
{
    public class PaymentReturnRequest
    {
        public string ResponseCode { get; set; } = string.Empty;
        public required string UserName { get; set; }
        public int? DiscountId { get; set; }
        public decimal? DiscountPrice { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Address { get; set; }
        public string? Description { get; set; }
    }
}
