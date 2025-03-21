using Microsoft.Extensions.Logging;
using Tea.Application.DTOs.Categories;
using Tea.Application.Mappers;
using Tea.Application.Services.Interfaces;
using Tea.Domain.Common;
using Tea.Domain.Constants;
using Tea.Domain.Entities;
using Tea.Domain.Exceptions;
using Tea.Domain.Exceptions.BadRequests;
using Tea.Domain.Exceptions.NotFounds;
using Tea.Domain.Repositories;

namespace Tea.Application.Services.Implements
{
    public class CategoryService(IUnitOfWork unit, ILogger<CategoryService> logger) : ICategoryService
    {
        public async Task<CategoryResponse> CreateChildrenAsync(CategoryCreateChildrenRequest request)
        {
            logger.LogInformation($"Creating a new children category with NAME: {request.Name}");

            // Tìm danh mục cha
            var parent = await unit.Category.FindAsync(x => x.Id == request.ParentId, tracked: true);

            if (parent == null)
            {
                logger.LogWarning($"Category Parent with ID: {request.ParentId} not found.");
                throw new CategoryNotFoundException(request.ParentId);
            }

            // Tạo danh mục con
            var children = new Category
            {
                Name = request.Name,
                Description = request.Description,
                Slug = request.Slug,
                ParentId = request.ParentId // Gán ParentId cho danh mục con
            };

            // Thêm danh mục con vào danh sách Children của danh mục cha
            parent.Children.Add(children);

            // Lưu thay đổi vào cơ sở dữ liệu
            if (await unit.SaveChangesAsync())
            {
                logger.LogInformation($"Children Category created successfully with ID: {children.Id}");

                // Xây dựng DisplayName bằng cách duyệt qua tất cả các cấp cha
                var displayName = await BuildDisplayName(children);

                // Trả về kết quả
                var entityToReturn = new CategoryResponse
                {
                    Id = children.Id,
                    Name = displayName, // Sử dụng DisplayName đã xây dựng
                    Description = children.Description,
                    Slug = children.Slug,
                };
                return entityToReturn;
            }

            logger.LogError($"{Logging.SaveChangesFailed}");
            throw new SaveChangesFailedException();
        }

        public async Task<CategoryResponse> CreateParentAsync(CategoryCreateParentRequest request)
        {
            logger.LogInformation($"Creating a new category with NAME: {request.Name}");

            var entity = new Category
            {
                Name = request.Name,
                Description = request.Description,
                Slug = request.Slug,
            };

            unit.Category.Add(entity);

            if (await unit.SaveChangesAsync())
            {
                logger.LogInformation($"Crategory created successfully with ID: {entity.Id}");
                return CategoryMapper.EntityToResponse(entity!);
            }

            logger.LogError($"{Logging.SaveChangesFailed}");
            throw new SaveChangesFailedException();
        }

        public async Task DeleteAsync(int id)
        {
            logger.LogInformation($"Deleting category with ID: {id}");

            var entity = await unit.Category.FindAsync(x => x.Id == id, tracked: true);
            if (entity == null)
            {
                logger.LogWarning($"Category with ID: {id} not found.");
                throw new CategoryNotFoundException(id);
            }

            entity.IsDeleted = true;

            if(!await unit.SaveChangesAsync())
            {
                logger.LogError($"{Logging.SaveChangesFailed}");
                throw new SaveChangesFailedException();
            }

            logger.LogInformation($"Category with ID: {id} was deleted.");
        }

        public async Task DeletesAsync(List<int> categoryIdList)
        {
            logger.LogInformation($"Deleting categories with IDs: {string.Join(", ", categoryIdList)}");

            var entities = await unit.Category.FindAllAsync(x => categoryIdList.Contains(x.Id), tracked: true);
            if (entities == null || !entities.Any())
            {
                logger.LogWarning("No categories found to delete.");
                throw new EmptyCategoryIdListException();
            }

            foreach (var entity in entities)
            {
                entity.IsDeleted = true;
            }

            if (!await unit.SaveChangesAsync())
            {
                logger.LogError($"{Logging.SaveChangesFailed}");
                throw new SaveChangesFailedException();
            }

            logger.LogInformation($"Categories with IDs: {string.Join(", ", categoryIdList)} were deleted.");
        }

        public async Task<IEnumerable<CategoryResponse>> GetAllAsync()
        {
            var allList = await unit.Category.FindAllAsync(predicate: null, tracked: false);
            return allList.Select(CategoryMapper.EntityToResponse);
        }

        public async Task<CategoryResponse> GetByIdAsync(int id)
        {
            logger.LogInformation($"Getting Category with ID: {id}.");

            var entity = await unit.Category.FindAsync(x => x.Id == id);
            if (entity == null)
            {
                logger.LogWarning($"Category with ID: {id} not found.");
                throw new CategoryNotFoundException(id);
            }

            logger.LogInformation($"Category with ID: {id} was retrieved.");
            return CategoryMapper.EntityToResponse(entity);
        }

        public async Task<PaginationResponse<CategoryResponse>> GetPaginationAsync(PaginationRequest request)
        {
            logger.LogInformation($"Getting categories with Pagination. PageIndex: {request.PageIndex}, Page Size: {request.PageSize}");
            var pagination = await unit.Category.GetPaginationAsync(request);
            var responseList = pagination.Data.Select(CategoryMapper.EntityToResponse);

            logger.LogInformation($"Successfully retrieved {pagination.Count} categories for Page: {pagination.PageIndex}");
            return new PaginationResponse<CategoryResponse>(pagination.PageIndex, pagination.PageSize, pagination.Count, responseList);
        }

        public async Task<CategoryResponse> UpdateAsync(int id, CategoryUpdateRequest request)
        {
            logger.LogInformation($"Updating Category with ID: {id}");

            if(id != request.Id)
            {
                logger.LogWarning($"{Logging.IdMismatch(routeId: id, bodyId: request.Id)}");
                throw new IdMismatchException("Vui lòng chọn lại danh mục cần cập nhật.");
            }

            var entity = await unit.Category.FindAsync(x => x.Id == request.Id, tracked: true);

            if(entity == null)
            {
                logger.LogWarning($"Category with ID: {id} not found.");
                throw new CategoryNotFoundException(request.Id);
            }

            entity.Name = request.Name;
            entity.Description = request.Description;
            entity.Slug = request.Slug;

            if(await unit.SaveChangesAsync())
            {
                logger.LogInformation($"Category updated successfully with ID: ${id}");
                // Xây dựng DisplayName bằng cách duyệt qua tất cả các cấp cha
                var displayName = await BuildDisplayName(entity);

                // Trả về kết quả
                var entityToReturn = new CategoryResponse
                {
                    Id = entity.Id,
                    Name = displayName, // Sử dụng DisplayName đã xây dựng
                    Description = entity.Description,
                    Slug = entity.Slug,
                };
                return entityToReturn;
            }

            logger.LogError($"{Logging.SaveChangesFailed}");
            throw new SaveChangesFailedException();


        }

        #region Helper
        private async Task<string> BuildDisplayName(Category category)
        {
            var displayName = category.Name;
            var currentCategory = category;

            // Duyệt qua tất cả các cấp cha
            while (currentCategory.ParentId.HasValue)
            {
                // Tìm danh mục cha
                var parent = await unit.Category.FindAsync(x => x.Id == currentCategory.ParentId.Value);

                if (parent == null)
                {
                    break; 
                }

                // Thêm tên của danh mục cha vào DisplayName
                displayName = $"{parent.Name} >> {displayName}";
                currentCategory = parent; 
            }

            return displayName;
        }
        #endregion
    }
}
