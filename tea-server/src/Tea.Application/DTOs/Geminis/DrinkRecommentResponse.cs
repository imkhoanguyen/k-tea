namespace Tea.Application.DTOs.Geminis
{
    public class DrinkRecommentResponse
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required string ImgUrl { get; set; }
        public required string Reason { get; set; }
    }
}
