using Microsoft.Extensions.Logging;
using Tea.Application.DTOs.Categories;
using Tea.Application.Mappers;
using Tea.Application.Services.Interfaces;
using Tea.Domain.Common;
using Tea.Domain.Constants;
using Tea.Domain.Entities;
using Tea.Domain.Exceptions;
using Tea.Domain.Repositories;

namespace Tea.Application.Services.Implements
{
    public class CategoryService(IUnitOfWork unit, ILogger<CategoryService> logger) : ICategoryService
    {
        public async Task<CategoryResponse> CreateChildrenAsync(CategoryCreateChildrenRequest request)
        {
            logger.LogInformation($"Creating a new children category with NAME: {request.Name}");

            var parent = await unit.Category.FindAsync(x => x.Id == request.ParentId, tracked: true);

            if (parent == null)
            {
                logger.LogWarning($"Category Parent with ID: {request.ParentId} not found.");
                throw new CategoryNotFoundException(request.ParentId);
            }

            var children = new Category
            {
                Name = request.Name,
                Description = request.Description,
                Slug = request.Slug,
            };

            parent.Children.Add(children);

            if (await unit.SaveChangesAsync())
            {
                logger.LogInformation($"Children Crategory created successfully with ID: {children.Id}");
                var entityToReturn = await unit.Category.FindAsync(x => x.Id == children.Id);
                return CategoryMapper.EntityToResponse(entityToReturn!);
            }

            logger.LogError($"{Logging.SaveChangesFailed}");
            throw new SaveChangesFailedException("Category");
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
                var entityToReturn = await unit.Category.FindAsync(x => x.Id == entity.Id);
                return CategoryMapper.EntityToResponse(entityToReturn!);
            }

            logger.LogError($"{Logging.SaveChangesFailed}");
            throw new SaveChangesFailedException("Category");
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
                throw new SaveChangesFailedException("Category");
            }

            logger.LogInformation($"Category with ID: {id} was deleted.");
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
                throw new IdMismatchException(routeId: id, bodyId: request.Id);
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
                var entityToReturn = await unit.Category.FindAsync(x => x.Id ==  entity.Id);
                return CategoryMapper.EntityToResponse(entityToReturn!);
            }

            logger.LogError($"{Logging.SaveChangesFailed}");
            throw new SaveChangesFailedException("Category");
        }
    }
}
