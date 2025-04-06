namespace Tea.Domain.Entities
{
    public class Discount
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public decimal? AmountOff { get; set; }
        public decimal? PercentOff { get; set; }
        public required string PromotionCode { get; set; }
        public bool IsDeleted { get; set; }
    }
}
