namespace Tea.Application.DTOs.Items
{
    public class ItemUpdateRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<int> CategoryIdList { get; set; } = [];
        public bool IsPublished { get; set; }
        public bool IsFeatured { get; set; }
    }
}
