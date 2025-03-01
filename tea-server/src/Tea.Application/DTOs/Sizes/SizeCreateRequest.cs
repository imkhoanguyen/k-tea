namespace Tea.Application.DTOs.Sizes
{
    public class SizeCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? NewPrice { get; set; }
    }
}
