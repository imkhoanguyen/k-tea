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
                logger.LogInformation("Add sizes to item successfully");
                return ItemMapper.EntityToResponse(entity);
            }

            logger.LogError(Logging.SaveChangesFailed);
            throw new SaveChangesFailedException("Item");
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
                throw new UploadFileFailedException();
            }

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
                };

                /*
                 * add item category
                 */
                foreach (var categoryId in request.CategoryIdList)
                {
                    var itemCategory = new ItemCategory
                    {
                        CategoryId = categoryId,
                        ItemId = item.Id,
                    };
                    item.ItemCategories.Add(itemCategory);
                }

                /*
                 * addd size 
                 */
                var sizeCreateRequests = JsonConvert.DeserializeObject<List<SizeCreateRequest>>(request.SizeCreateRequestJson);

                if (sizeCreateRequests != null)
                {
                    foreach (var sizeRequest in sizeCreateRequests)
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
                }

                unit.Item.Add(item);

                if (await unit.SaveChangesAsync())
                {
                    logger.LogInformation($"Crategory created successfully with ID: {item.Id}");
                    return ItemMapper.EntityToResponse(item);
                }

                logger.LogError(Logging.SaveChangesFailed);
                throw new SaveChangesFailedException("Item");
            }
            catch
            {
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
                throw new SaveChangesFailedException("Item");
            }
            logger.LogInformation($"Item with ID: {id} was deleted.");
        }

        public async Task DeleteSizesAsync(int itemId, List<int> sizeIdList)
        {
            logger.LogInformation($"delete size in item with itemID: {itemId}");

            if(sizeIdList == null || sizeIdList.Count == 0)
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

            if(item.Sizes.Count - sizesToDelete.Count < 1)
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
                throw new SaveChangesFailedException("Item");
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

        public async Task<PaginationResponse<ItemResponse>> GetPaginationAsync(PaginationRequest request)
        {
            logger.LogInformation($"Retrieving items with Pagination. PageIndex: {request.PageIndex}, Page Size: {request.PageSize}");
            var pagination = await unit.Item.GetPaginationAsync(request, expression: null);
            var responseList = pagination.Select(ItemMapper.EntityToResponse);

            logger.LogInformation($"Successfully retrieved {pagination.Count} items for Page: {pagination.CurrentPage}");
            return new PaginationResponse<ItemResponse>(responseList, pagination.Count, pagination.CurrentPage, pagination.PageSize);
        }

        public async Task<PaginationResponse<ItemResponse>> GetPublicPaginationAsync(PaginationRequest request)
        {
            logger.LogInformation($"Retrieving public items with Pagination. PageIndex: {request.PageIndex}, Page Size: {request.PageSize}");
            var pagination = await unit.Item.GetPaginationAsync(request, x => x.IsPublished);
            var responseList = pagination.Select(ItemMapper.EntityToResponse);

            logger.LogInformation($"Successfully retrieved public {pagination.Count} items for Page: {pagination.CurrentPage}");
            return new PaginationResponse<ItemResponse>(responseList, pagination.Count, pagination.CurrentPage, pagination.PageSize);
        }

        public async Task<string> UpdateImageAsync(int itemId, IFormFile imgFile)
        {
            logger.LogInformation($"Updating image of item with ID: {itemId}");

            if(imgFile == null || imgFile.Length == 0)
            {
                logger.LogWarning("File request cant empty or null");
                throw new EmptyFileRequestException();
            }

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

            try
            {
                // delete current image of item (cloudinary)
                await cloudinaryService.DeleteFileAsync(entity.PublicId!);

                // change new url and publicId 
                entity.PublicId = resultUploadImage.PublicId;
                entity.ImgUrl = resultUploadImage.Url!;

                if (await unit.SaveChangesAsync())
                {
                    logger.LogInformation($"Successfully updated image of item with ID: {itemId}");
                    return entity.ImgUrl;
                }

                logger.LogWarning(Logging.SaveChangesFailed);
                throw new SaveChangesFailedException("Item");

            }
            catch
            {
                // rollback: delete image new uploaded
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
                throw new IdMismatchException(routeId: id, bodyId: request.Id);
            }

            if (request.CategoryIdList.Count == 0)
            {
                logger.LogWarning("Item must have at least one category.");
                throw new ItemMustHaveAtLeastOneCategoryException();
            }

            var entity = await unit.Item.FindAsync(x => x.Id == id, tracked: true);
            if (entity == null)
            {
                logger.LogWarning($"Item with ID: {id} not found.");
                throw new ItemNotFoundException(id);
            }

            entity.Name = request.Name;
            entity.Description = request.Description;
            entity.Slug = request.Slug;

            var itemCategoriesRequest = request.CategoryIdList.Select(categoryId => new ItemCategory
            {
                CategoryId = categoryId,
                ItemId = entity.Id,
            });

            var currentCategoriesItem = entity.ItemCategories.ToList();

            //add new category to item
            var categoriesNeedAdd = itemCategoriesRequest
                .Select(x => new ItemCategory { CategoryId = x.CategoryId, ItemId = x.ItemId})
                .Except(currentCategoriesItem);
            entity.ItemCategories.AddRange(categoriesNeedAdd);

            //delete category in item
            var categoriesNeedDelete = currentCategoriesItem
                .Except(itemCategoriesRequest.Select(x => new ItemCategory { CategoryId = x.CategoryId, ItemId = x.ItemId })); 
            
            foreach (var itemCategory in categoriesNeedDelete)
            {
                entity.ItemCategories.Remove(itemCategory);
            }

            if (await unit.SaveChangesAsync())
            {
                logger.LogInformation($"Successfully updated item with ID: {id}");
                // mặc dù đã tracked entity nhưng do có thêm category mới vào nó vẫn track được nhưng ko include được category
                // => get lại
                var entityToReturn = await unit.Item.FindAsync(x => x.Id == entity.Id);
                return ItemMapper.EntityToResponse(entityToReturn!);
            }

            logger.LogError(Logging.SaveChangesFailed);
            throw new SaveChangesFailedException("Item");
        }

        // update size info of item
        public async Task<ItemResponse> UpdateSizeAsync(int itemId, SizeUpdateRequest request)
        {
            logger.LogInformation($"updating size of item with itemId: {itemId}");
            if (itemId != request.ItemId)
            {
                logger.LogWarning(Logging.IdMismatch(routeId: itemId, request.ItemId));
                throw new IdMismatchException(routeId: itemId, bodyId: request.ItemId);
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
            throw new SaveChangesFailedException("Item");
        }
    }
}
