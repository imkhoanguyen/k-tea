namespace Tea.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public bool IsDeleted { get; set; }
        public required string Slug { get; set; }
        public int? ParentId { get; set; }
        public Category? Parent { get; set; }
        public List<Category> Children { get; set; } = [];
        public List<ItemCategory> ItemCategories { get; set; } = [];
    }
}
