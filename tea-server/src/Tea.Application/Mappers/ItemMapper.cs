using Tea.Application.DTOs.Items;
using Tea.Domain.Entities;

namespace Tea.Application.Mappers
{
    public class ItemMapper
    {
        public static ItemResponse EntityToResponse(Item item)
        {
            return new ItemResponse
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description ?? "",
                Slug = item.Slug,
                ImgUrl = item.ImgUrl,
                IsPublished = item.IsPublished,
                IsFeatured = item.IsFeatured,
                Sizes = item.Sizes.Select(SizeMapper.EntityToResponse).ToList(),
                Categories = item.ItemCategories.Where(x => x.Category != null)
                .Select(x => CategoryMapper.EntityToResponse(x.Category!)).ToList(),
            };
        }
    }
}
