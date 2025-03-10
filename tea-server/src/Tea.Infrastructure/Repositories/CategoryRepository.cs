using System.Linq.Expressions;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Tea.Domain.Common;
using Tea.Domain.Entities;
using Tea.Domain.Repositories;
using Tea.Infrastructure.DataAccess;
using Tea.Infrastructure.Utilities;

namespace Tea.Infrastructure.Repositories
{
    public class CategoryRepository(TeaContext context) : Repository<Category>(context), ICategoryRepository
    {
        public async Task<PaginationResponse<Category>> GetPaginationAsync(PaginationRequest request)
        {
            var query = context.Categories
                .AsNoTracking()
                .Where(x => !x.IsDeleted).AsQueryable();

            if (!string.IsNullOrEmpty(request.Search))
            {
                query = query.Where(x => x.Name.ToLower().Contains(request.Search)
                || x.Id.ToString().Contains(request.Search));
            }

            if (!string.IsNullOrEmpty(request.OrderBy))
            {
                query = request.OrderBy switch
                {
                    "id" => query.OrderBy(x => x.Id),
                    "id_desc" => query.OrderByDescending(x => x.Id),
                    "name" => query.OrderBy(x => x.Name),
                    "name_desc" => query.OrderByDescending(x => x.Name),
                    _ => query.OrderByDescending(x => x.Id)
                };
            }

            var allCategories = await query.ToListAsync();

            // Tạo từ điển để lưu trữ danh mục theo Id
            var categoryDictionary = allCategories.ToDictionary(x => x.Id);

            var categoriesList = new List<Category>();
            foreach (var category in allCategories)
            {
                var categoryListItem = new Category
                {
                    Id = category.Id,
                    Name = BuildDisplayName(category, categoryDictionary), 
                    ParentId = category.ParentId,
                    Description = category.Description,
                    Slug = category.Slug,
                };

                categoriesList.Add(categoryListItem);
            }

            return categoriesList.ApplyPagination(request.PageIndex, request.PageSize);
        }

        #region Helper
        private string BuildDisplayName(Category category, Dictionary<int, Category> categoryDictionary)
        {
            var displayName = category.Name;
            var parentId = category.ParentId;

            while (parentId.HasValue && categoryDictionary.TryGetValue(parentId.Value, out var parentCategory))
            {
                displayName = $"{parentCategory.Name} >> {displayName}";
                parentId = parentCategory.ParentId;
            }

            return displayName;
        }
        #endregion
    }
}
