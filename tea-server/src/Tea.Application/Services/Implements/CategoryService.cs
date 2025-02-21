using Tea.Application.DTOs.Categories;
using Tea.Application.Mappers;
using Tea.Application.Services.Interfaces;
using Tea.Domain.Common;
using Tea.Domain.Entities;
using Tea.Domain.Exceptions;
using Tea.Domain.Repositories;

namespace Tea.Application.Services.Implements
{
    public class CategoryService(IUnitOfWork unit) : ICategoryService
    {
        public async Task<CategoryResponse> CreateAsync(CategoryCreateRequest request)
        {
            var entity = new Category
            {
                Name = request.Name,
                Description = request.Description,
                Slug = request.Slug,
                ParentId = request.ParentId,
            };

            unit.Category.Add(entity);

            if (await unit.SaveChangesAsync())
            {
                return CategoryMapper.EntityToResponse(entity);
            }

            throw new Exception("Problem create category.");
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await unit.Category.FindAsync(x => x.Id == id, tracked: true);
            if (entity == null)
                throw new CategoryNotFoundException(id);

            entity.IsDeleted = true;

            if(!await unit.SaveChangesAsync())
            {
                throw new Exception($"Problem delete category with id: {id}.");
            }
        }

        public async Task<CategoryResponse> GetByIdAsync(int id)
        {
            var entity = await unit.Category.FindAsync(x => x.Id == id);
            if (entity == null)
                throw new CategoryNotFoundException(id);
            return CategoryMapper.EntityToResponse(entity);
        }

        public async Task<PaginationResponse<CategoryResponse>> GetPaginationAsync(PaginationRequest request)
        {
            var pagination = await unit.Category.GetPaginationAsync(request);
            var responseList = pagination.Select(CategoryMapper.EntityToResponse);
            return new PaginationResponse<CategoryResponse>(responseList, pagination.Count, pagination.CurrentPage, pagination.PageSize);
        }

        public async Task<CategoryResponse> UpdateAsync(CategoryUpdateRequest request)
        {
            var entity = await unit.Category.FindAsync(x => x.Id == request.Id, tracked: true);

            if(entity == null)
                throw new CategoryNotFoundException(request.Id);

            entity.Name = request.Name;
            entity.Description = request.Description;
            entity.Slug = request.Slug;

            if(await unit.SaveChangesAsync())
            {
                var entityToReturn = await unit.Category.FindAsync(x => x.Id ==  entity.Id);
                return CategoryMapper.EntityToResponse(entityToReturn!);
            }

            throw new Exception($"Problem update category with id: {request.Id}.");
        }
    }
}
