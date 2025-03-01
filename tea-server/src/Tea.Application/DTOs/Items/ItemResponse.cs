using Tea.Application.DTOs.Categories;
using Tea.Application.DTOs.Sizes;

namespace Tea.Application.DTOs.Items
{
    public class ItemResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string ImgUrl { get; set; } = string.Empty;
        public bool IsPublished { get; set; }
        public List<CategoryResponse> Categories { get; set; } = [];
        public List<SizeResponse> Sizes { get; set; } = [];
    }
}
