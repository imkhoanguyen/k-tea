namespace Tea.Application.DTOs.Discounts
{
    public class DiscountUpdateRequest
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public decimal? AmountOff { get; set; }
        public decimal? PercentOff { get; set; }
        public required string PromotionCode { get; set; }
    }
}
