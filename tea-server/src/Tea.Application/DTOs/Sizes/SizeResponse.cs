namespace Tea.Application.DTOs.Sizes
{
    public class SizeResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal NewPrice { get; set; }
    }
}
