namespace Tea.Domain.Entities
{
    public class Item
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required string Slug { get; set; }
        public required string ImgUrl { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsPublished { get; set; }
        public List<ItemCategory> ItemCategories { get; set; } = [];
        public List<Size> Sizes { get; set; } = [];
    }
}
