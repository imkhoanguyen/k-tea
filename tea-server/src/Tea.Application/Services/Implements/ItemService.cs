using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Tea.Application.DTOs.Items;
using Tea.Application.DTOs.Sizes;
using Tea.Application.Interfaces;
using Tea.Application.Mappers;
using Tea.Application.Services.Interfaces;
using Tea.Domain.Common;
using Tea.Domain.Entities;
using Tea.Domain.Exceptions;
using Tea.Domain.Repositories;

namespace Tea.Application.Services.Implements
{
    public class ItemService(IUnitOfWork unit, ICloudinaryService cloudinaryService, ILogger<ItemService> logger) : IItemService
    {
        public async Task<ItemResponse> CreateAsync(ItemCreateRequest request)
        {
            /*
             * upload img to cloudinary
             */
            var resultUploadImg = await cloudinaryService.AddImageAsync(request.ImgFile);

            if (resultUploadImg.Error != null)
            {
                throw new UploadFileFailedException();
            }

            var item = new Item
            {
                Name = request.Name,
                Description = request.Description,
                Slug = request.Slug,
                IsPublished = request.IsPublished,
                ImgUrl = resultUploadImg.Url!,
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
                        ItemId = item.Id
                    };
                    item.Sizes.Add(size);
                }
            }

            unit.Item.Add(item);

            if (await unit.SaveChangesAsync())
            {
                var entityToReturn = await unit.Item.FindAsync(x => x.Id == item.Id);
                return ItemMapper.EntityToResponse(entityToReturn!);
            }

            throw new SaveChangesFailedException("Item");
        }

        public async Task<ItemResponse> GetByIdAsync(int id)
        {
            var entity = await unit.Item.FindAsync(x => x.Id == id);

            if(entity == null)
            {
                throw new ItemNotFoundException(id);
            }

            return ItemMapper.EntityToResponse(entity);
        }

        public Task<PaginationResponse<ItemResponse>> GetPaginationAsync(PaginationRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
