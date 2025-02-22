using System.ComponentModel.DataAnnotations.Schema;

namespace Tea.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public bool IsDeleted { get; set; } = false;
        public required string Slug { get; set; }
        public int? ParentId { get; set; }
        public List<Category> Children { get; set; } = [];
    }
}
