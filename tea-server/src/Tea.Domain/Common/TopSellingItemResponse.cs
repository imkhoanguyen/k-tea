namespace Tea.Domain.Common
{
    public class TopSellingItemResponse
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string ImgUrl { get; set; }
        public int TotalSold { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
