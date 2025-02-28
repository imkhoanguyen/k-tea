using Tea.Application.DTOs.Categories;
using Tea.Domain.Entities;

namespace Tea.Application.Mappers
{
    public class CategoryMapper
    {
        public static CategoryResponse EntityToResponse(Category entity)
        {
            return new CategoryResponse
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Slug = entity.Slug,
                Children = entity.Children.Select(EntityToResponse).ToList() ?? [],
            };
        }
    }
}
