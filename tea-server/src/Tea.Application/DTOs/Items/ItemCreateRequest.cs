using Microsoft.AspNetCore.Http;
using Tea.Application.DTOs.Sizes;

namespace Tea.Application.DTOs.Items
{
    public class ItemCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public bool IsPublished { get; set; }
        public required IFormFile ImgFile { get; set; }
        public List<int> CategoryIdList { get; set; } = [];
        public string SizeCreateRequestJson { get; set; } = string.Empty;
    }
}
