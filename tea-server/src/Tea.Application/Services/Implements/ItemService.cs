using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Tea.Application.DTOs.Items;
using Tea.Application.DTOs.Sizes;
using Tea.Application.Interfaces;
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
    public class ItemService(IUnitOfWork unit, ICloudinaryService cloudinaryService, ILogger<ItemService> logger) : IItemService
    {
        public async Task<ItemResponse> AddSizesAsync(int itemId, List<SizeCreateRequest> requests)
        {
            logger.LogInformation($"Add size to item with itemID: {itemId}");
            if (requests == null || requests.Count == 0)
            {
                logger.LogWarning("List size requests cannot be null or empty.");
                throw new EmptySizeListException();
            }

            var entity = await unit.Item.FindAsync(x => x.Id == itemId, tracked: true);
            if (entity == null)
            {
                logger.LogWarning($"Item with ID: {itemId} not found.");
                throw new ItemNotFoundException(itemId);
            }

            await unit.BeginTransactionAsync();

            try
            {
                foreach (var sizeCreateRequest in requests)
                {
                    var size = new Size
                    {
                        Name = sizeCreateRequest.Name,
                        Description = sizeCreateRequest.Description,
                        Price = sizeCreateRequest.Price,
                        NewPrice = sizeCreateRequest.NewPrice,
                    };

                    entity.Sizes.Add(size);
                }

                if (await unit.SaveChangesAsync())
                {
                    await unit.CommitTransactionAsync();
                    logger.LogInformation("Add sizes to item successfully");
                    return ItemMapper.EntityToResponse(entity);
                }

                logger.LogError(Logging.SaveChangesFailed);
                throw new SaveChangesFailedException();
            } catch
            {
                await unit.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<ItemResponse> CreateAsync(ItemCreateRequest request)
        {
            logger.LogInformation($"Creating a new item with NAME: {request.Name}");
            /*
             * upload img to cloudinary
             */

            var resultUploadImg = await cloudinaryService.AddImageAsync(request.ImgFile);

            if (resultUploadImg.Error != null)
            {
                throw new UploadFileFailedException(resultUploadImg.Error);
            }

            await unit.BeginTransactionAsync();

            try
            {
                var item = new Item
                {
                    Name = request.Name,
                    Description = request.Description,
                    Slug = request.Slug,
                    IsPublished = request.IsPublished,
                    ImgUrl = resultUploadImg.Url!,
                    PublicId = resultUploadImg.PublicId,
                    IsFeatured = request.IsFeatured,
                };

                /*
                 * add item category
                 */
                foreach (var categoryId in request.CategoryIdList)
                {
                    var itemCategory = new ItemCategory(item.Id, categoryId);
                    item.ItemCategories.Add(itemCategory);
                }

                // add size
                var sizeCreateRequests = JsonConvert.DeserializeObject<List<SizeCreateRequest>>(request.SizeCreateRequestJson);

                foreach (var sizeRequest in sizeCreateRequests!)
                {
                    var size = new Size
                    {
                        Name = sizeRequest.Name,
                        Description = sizeRequest.Description,
                        Price = sizeRequest.Price,
                        NewPrice = sizeRequest.NewPrice,
                    };
                    item.Sizes.Add(size);
                }

                unit.Item.Add(item);

                if (await unit.SaveChangesAsync())
                {
                    await unit.CommitTransactionAsync();
                    logger.LogInformation($"Crategory created successfully with ID: {item.Id}");
                    // do chỉ có categoryId nên phải get lại để get thông tin category 
                    // => get lại
                    var entityToReturn = await unit.Item.FindAsync(x => x.Id == item.Id);
                    return ItemMapper.EntityToResponse(entityToReturn!);
                }

                logger.LogError(Logging.SaveChangesFailed);
                throw new SaveChangesFailedException();
            }
            catch
            {
                await unit.RollbackTransactionAsync();
                await cloudinaryService.DeleteFileAsync(resultUploadImg.PublicId!);
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            logger.LogInformation($"Deleting item with ID: {id}");
            var entity = await unit.Item.FindAsync(x => x.Id == id, tracked: true);
            if (entity == null)
            {
                logger.LogWarning($"Item with ID: {id} not found.");
                throw new ItemNotFoundException(id);
            }

            entity.IsDeleted = true;
            entity.IsPublished = false;

            if (!await unit.SaveChangesAsync())
            {
                logger.LogError($"{Logging.SaveChangesFailed}");
                throw new SaveChangesFailedException();
            }
            logger.LogInformation($"Item with ID: {id} was deleted.");
        }

        public async Task DeletesAsync(List<int> itemIdList)
        {
            logger.LogInformation($"Deleting items with IDs: {string.Join(", ", itemIdList)}");

            var entities = await unit.Item.FindAllAsync(x => itemIdList.Contains(x.Id), tracked: true);
            if (entities == null || !entities.Any())
            {
                logger.LogWarning("No items found to delete.");
                throw new EmptyItemListException();
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

            logger.LogInformation($"Items with IDs: {string.Join(", ", itemIdList)} were deleted.");
        }

        public async Task DeleteSizesAsync(int itemId, List<int> sizeIdList)
        {
            logger.LogInformation($"delete size in item with itemID: {itemId}");

            if (sizeIdList == null || sizeIdList.Count == 0)
            {
                logger.LogWarning("List sizeId cannot be null or empty.");
                throw new EmptySizeListException();
            }

            var item = await unit.Item.FindAsync(x => x.Id == itemId, tracked: true);
            if (item == null)
            {
                logger.LogWarning($"Item with ID: {itemId} not found.");
                throw new ItemNotFoundException(itemId);
            }

            var sizesToDelete = item.Sizes.Where(x => sizeIdList.Contains(x.Id)).ToList();

            if (item.Sizes.Count - sizesToDelete.Count < 1)
            {
                logger.LogWarning("Cannot delete sizes because the item must have at least one size.");
                throw new ItemMustHaveAtLeastOneSizeException();
            }

            foreach (var size in sizesToDelete)
            {
                item.Sizes.Remove(size);
            }

            if (!await unit.SaveChangesAsync())
            {
                logger.LogError(Logging.SaveChangesFailed);
                throw new SaveChangesFailedException();
            }

            logger.LogInformation("Delete size in item successfully");
        }

        public async Task<ItemResponse> GetByIdAsync(int id)
        {
            logger.LogInformation($"Retrieving item with ID: {id}");
            var entity = await unit.Item.FindAsync(x => x.Id == id);

            if (entity == null)
            {
                logger.LogWarning($"Item with ID: {id} not found.");
                throw new ItemNotFoundException(id);
            }

            logger.LogInformation($"Successfully retrieved item with ID: {id}");
            return ItemMapper.EntityToResponse(entity);
        }

        public async Task<PaginationResponse<ItemResponse>> GetPaginationAsync(ItemPaginationRequest request)
        {
            logger.LogInformation($"Retrieving items with Pagination. PageIndex: {request.PageIndex}, Page Size: {request.PageSize}");
            var pagination = await unit.Item.GetPaginationAsync(request, expression: null);
            var responseList = pagination.Data.Select(ItemMapper.EntityToResponse);

            logger.LogInformation($"Successfully retrieved {pagination.Count} items for Page: {pagination.PageIndex}");
            return new PaginationResponse<ItemResponse>(pagination.PageIndex, pagination.PageSize, pagination.Count, responseList);
        }

        public async Task<PaginationResponse<ItemResponse>> GetPublicPaginationAsync(ItemPaginationRequest request)
        {
            logger.LogInformation($"Retrieving public items with Pagination. PageIndex: {request.PageIndex}, Page Size: {request.PageSize}");
            var pagination = await unit.Item.GetPaginationAsync(request, x => x.IsPublished);
            var responseList = pagination.Data.Select(ItemMapper.EntityToResponse);

            logger.LogInformation($"Successfully retrieved public {pagination.Count} items for Page: {pagination.PageIndex}");
            return new PaginationResponse<ItemResponse>(pagination.PageIndex, pagination.PageSize, pagination.Count, responseList);
        }

        public async Task<string> UpdateImageAsync(int itemId, IFormFile imgFile)
        {
            logger.LogInformation($"Updating image of item with ID: {itemId}");

            var entity = await unit.Item.FindAsync(x => x.Id == itemId, tracked: true);
            if (entity == null)
            {
                logger.LogWarning($"Item with ID: {itemId} not found.");
                throw new ItemNotFoundException(itemId);
            }

            var resultUploadImage = await cloudinaryService.AddImageAsync(imgFile);
            if (resultUploadImage.Error != null)
            {
                throw new UploadFileFailedException();
            }

            await unit.BeginTransactionAsync();

            try
            {
                // delete current image of item (cloudinary)
                await cloudinaryService.DeleteFileAsync(entity.PublicId!);

                // change new url and publicId 
                entity.PublicId = resultUploadImage.PublicId;
                entity.ImgUrl = resultUploadImage.Url!;

                if (await unit.SaveChangesAsync())
                {
                    await unit.CommitTransactionAsync();
                    logger.LogInformation($"Successfully updated image of item with ID: {itemId}");
                    return entity.ImgUrl;
                }

                logger.LogWarning(Logging.SaveChangesFailed);
                throw new SaveChangesFailedException();

            }
            catch
            {
                // rollback: delete image new uploaded
                await unit.RollbackTransactionAsync();
                await cloudinaryService.DeleteFileAsync(resultUploadImage.PublicId!);
                throw;
            }
        }

        // update item info and category of item
        public async Task<ItemResponse> UpdateItemAsync(int id, ItemUpdateRequest request)
        {
            logger.LogInformation($"Updating item with ID: {id}");
            if (id != request.Id)
            {
                logger.LogWarning(Logging.IdMismatch(routeId: id, bodyId: request.Id));
                throw new IdMismatchException("Vui lòng chọn lại sản phẩm cần cập nhật");
            }

            if (request.CategoryIdList.Count == 0)
            {
                logger.LogWarning("Item must have at least one category.");
                throw new ItemMustHaveAtLeastOneCategoryException();
            }

            await unit.BeginTransactionAsync();

            try
            {
                var entity = await unit.Item.FindAsync(x => x.Id == id, tracked: true);
                if (entity == null)
                {
                    logger.LogWarning($"Item with ID: {id} not found.");
                    throw new ItemNotFoundException(id);
                }

                entity.Name = request.Name;
                entity.Description = request.Description;
                entity.Slug = request.Slug;
                entity.IsFeatured = request.IsFeatured;
                entity.IsPublished = request.IsPublished;

                var categoryIdRequest = request.CategoryIdList;

                var currentCategoriesItem = entity.ItemCategories.ToList();

                // get exits list categoryId
                // chuyen sang hashset giup truy van nhanh hon khi so voi khi su dung list (existingCategoryIds.Contains)
                var existingCategoryIds = currentCategoriesItem.Select(x => x.CategoryId).ToHashSet();

                // itemcategory list need add
                var categoriesNeedAdd = categoryIdRequest
                    .Where(categoryId => !existingCategoryIds.Contains(categoryId))
                    .Select(categoryId => new ItemCategory(entity.Id, categoryId))
                    .ToList();

                if (categoriesNeedAdd.Count > 0)
                    unit.ItemCategory.AddRange(categoriesNeedAdd);

                // itemcategory list need remove
                var categoriesNeedDelete = currentCategoriesItem
                    .Where(ic => !categoryIdRequest.Contains(ic.CategoryId))
                    .ToList();

                if (categoriesNeedDelete.Count > 0)
                    unit.ItemCategory.RemoveRange(categoriesNeedDelete);

                if (await unit.SaveChangesAsync())
                {
                    await unit.CommitTransactionAsync();
                    logger.LogInformation($"Successfully updated item with ID: {id}");
                    
                    var entityToReturn = await unit.Item.FindAsync(x => x.Id == entity.Id);
                    return ItemMapper.EntityToResponse(entityToReturn!);
                }

                logger.LogError(Logging.SaveChangesFailed);
                throw new SaveChangesFailedException();
            } catch
            {
                await unit.RollbackTransactionAsync();
                throw;
            }

            
        }


        // update size info of item
        public async Task<ItemResponse> UpdateSizeAsync(int itemId, SizeUpdateRequest request)
        {
            logger.LogInformation($"updating size of item with itemId: {itemId}");
            if (itemId != request.ItemId)
            {
                logger.LogWarning(Logging.IdMismatch(routeId: itemId, request.ItemId));
                throw new IdMismatchException("Vui lòng chọn lại sản phẩm của size cần cập nhật.");
            }


            var item = await unit.Item.FindAsync(x => x.Id == itemId, tracked: true);
            if (item == null)
            {
                logger.LogWarning($"Item with ID: {itemId} not found.");
                throw new ItemNotFoundException(itemId);
            }

            var size = item.Sizes.FirstOrDefault(x => x.Id == request.Id);
            if (size == null)
            {
                logger.LogWarning($"Size with ID: {request.Id} not found in Item with ID: {itemId}.");
                throw new SizeNotFoundException(request.Id);
            }

            size.Name = request.Name;
            size.Description = request.Description;
            size.Price = request.Price;
            size.NewPrice = request.NewPrice;

            if (await unit.SaveChangesAsync())
            {
                logger.LogInformation($"Successfully updated size of item with ID: {request.Id}");
                return ItemMapper.EntityToResponse(item);
            }

            logger.LogError(Logging.SaveChangesFailed);
            throw new SaveChangesFailedException();
        }
    }
}
