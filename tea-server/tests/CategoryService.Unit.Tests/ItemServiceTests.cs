using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Tea.Application.DTOs.Cloudinaries;
using Tea.Application.DTOs.Items;
using Tea.Application.DTOs.Sizes;
using Tea.Application.Interfaces;
using Tea.Application.Services.Implements;
using Tea.Application.Services.Interfaces;
using Tea.Domain.Common;
using Tea.Domain.Entities;
using Tea.Domain.Exceptions;
using Tea.Domain.Repositories;
using Xunit;

namespace Tea.Application.Unit.Tests
{
    public class ItemServiceTests
    {
        private readonly IItemService _sut;
        private readonly Mock<IUnitOfWork> _unitMock;
        private readonly Mock<ILogger<ItemService>> _loggerMock;
        private readonly Mock<ICloudinaryService> _cloudinaryMock;

        public ItemServiceTests()
        {
            _unitMock = new Mock<IUnitOfWork>();
            _cloudinaryMock = new Mock<ICloudinaryService>();
            _loggerMock = new Mock<ILogger<ItemService>>();
            _sut = new ItemService(_unitMock.Object, _cloudinaryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async void AddSizesAsync_ShouldReturnItemResponse_WhenSizesAddedToItem()
        {
            // Arrange
            int itemId = 1;
            var requests = new List<SizeCreateRequest>
            {
                new SizeCreateRequest {Name = "size1", Description = "des 1", NewPrice = null, Price = 1},
                new SizeCreateRequest {Name = "size2", Description = "des 2", NewPrice = null, Price = 2},
                new SizeCreateRequest {Name = "size3", Description = "des 3", NewPrice = null, Price = 3},
            };

            var item = new Item
            {
                Id = itemId,
                Name = "item",
                Description = "item",
                Slug = "item",
                ImgUrl = "item",
                Sizes = [],
                ItemCategories = [],
            };

            _unitMock.Setup(x => x.Item.FindAsync(It.IsAny<Expression<Func<Item, bool>>>(), true))
                .ReturnsAsync(item);

            _unitMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(true);

            // Act
            var itemResponse = await _sut.AddSizesAsync(itemId, requests);

            // Assert
            Assert.NotNull(itemResponse);
            Assert.Equal(itemId, itemResponse.Id);
            Assert.Equal(3, itemResponse.Sizes.Count);
        }

        [Fact]
        public async void AddSizesAsync_ShouldThrowEmptySizeListException_WhenSizeListEmpty()
        {
            // Arrange
            int itemId = 1;
            var requests = new List<SizeCreateRequest>();

            var item = new Item
            {
                Id = itemId,
                Name = "item",
                Description = "item",
                Slug = "item",
                ImgUrl = "item",
                Sizes = [],
                ItemCategories = [],
            };

            // Act && Assert
            await Assert.ThrowsAsync<EmptySizeListException>(() => _sut.AddSizesAsync(itemId, requests));
        }

        [Fact]
        public async void AddSizesAsync_ShouldThrowItemNotFoundException_WhenItemNotFound()
        {
            // Arrange
            int itemId = 1;
            var requests = new List<SizeCreateRequest>();

            _unitMock.Setup(x => x.Item.FindAsync(It.IsAny<Expression<Func<Item, bool>>>(), true))
                .ReturnsAsync((Item)null);


            // Act && Assert
            await Assert.ThrowsAsync<EmptySizeListException>(() => _sut.AddSizesAsync(itemId, requests));
        }

        [Fact]
        public async void AddSizesAsync_ShouldThrowSaveChangeFailedException_WhenSaveChangeFailed()
        {
            // Arrange
            int itemId = 1;
            var requests = new List<SizeCreateRequest>
            {
                new SizeCreateRequest {Name = "size1", Description = "des 1", NewPrice = null, Price = 1},
                new SizeCreateRequest {Name = "size2", Description = "des 2", NewPrice = null, Price = 2},
                new SizeCreateRequest {Name = "size3", Description = "des 3", NewPrice = null, Price = 3},
            };

            var item = new Item
            {
                Id = itemId,
                Name = "item",
                Description = "item",
                Slug = "item",
                ImgUrl = "item",
                Sizes = [],
                ItemCategories = [],
            };

            _unitMock.Setup(x => x.Item.FindAsync(It.IsAny<Expression<Func<Item, bool>>>(), true))
                .ReturnsAsync(item);

            _unitMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(false);

            // Act &&  Assert
            await Assert.ThrowsAsync<SaveChangesFailedException>(() => _sut.AddSizesAsync(itemId, requests));
        }


        [Fact]
        public async void CreateAsync_ShouldReturnItemResponse_WhenItemCreated()
        {
            // Arrange
            var request = new ItemCreateRequest
            {
                Name = "item",
                Description = "item",
                Slug = "item",
                ImgFile = new Mock<IFormFile>().Object,
                SizeCreateRequestJson = @"[{""name"":""S"",""price"":10.0,""newPrice"":8.0, ""description"": ""des""},{""name"":""M"",""price"":12.0,""newPrice"":10.0,""description"":""des""}]",
                CategoryIdList = [1, 2]
            };

            var uploadResult = new FileUploadResult
            {
                Url = "https://static.nutscdn.com/vimg/0-0/71ee65b75262619d4fc6a431bcd20802.jpg",
                PublicId = "public_id",
                Error = null
            };

            var sizeCreateRequests = JsonConvert.DeserializeObject<List<SizeCreateRequest>>(request.SizeCreateRequestJson);
            var sizes = sizeCreateRequests?.Select(sizeRequest => new Size
            {
                Name = sizeRequest.Name,
                Description = sizeRequest.Description,
                Price = sizeRequest.Price,
                NewPrice = sizeRequest.NewPrice
            }).ToList();

            var categories = new List<Category>
            {
                new Category {Id=1, Name = "category", Description = "category", Slug = "category" },
                new Category {Id=2, Name = "category2", Description = "category2", Slug = "category2" },
            };

            var entity = new Item
            {
                Id = 1,
                Name = request.Name,
                Description = request.Description,
                Slug = request.Slug,
                ItemCategories = categories.Select(x => new ItemCategory { Category = x, ItemId = 1, CategoryId = x.Id }).ToList(),
                PublicId = uploadResult.PublicId,
                ImgUrl = uploadResult.Url,
                Sizes = sizes ?? []
            };

            _cloudinaryMock.Setup(x => x.AddImageAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync(uploadResult);

            _unitMock.Setup(x => x.Item.Add(entity));

            _unitMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(true);

            _unitMock.Setup(x => x.Item.FindAsync(It.IsAny<Expression<Func<Item, bool>>>(), false))
                .ReturnsAsync(entity);

            // Act
            var result = await _sut.CreateAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.Name, result.Name);
            Assert.Equal(request.Description, result.Description);
            Assert.Equal(request.Slug, result.Slug);
            Assert.Equal(uploadResult.Url, result.ImgUrl);
            Assert.Equal(2, result.Sizes.Count);
            Assert.Equal(2, result.Categories.Count);
        }

        [Fact]
        public async void CreateAsync_ShouldThrowUploadFileFailedException_WhenUploadImageFailed()
        {
            // Arrange
            var request = new ItemCreateRequest
            {
                Name = "item",
                Description = "item",
                Slug = "item",
                ImgFile = new Mock<IFormFile>().Object,
                SizeCreateRequestJson = @"[{""name"":""S"",""price"":10.0,""newPrice"":8.0, ""description"": ""des""},{""name"":""M"",""price"":12.0,""newPrice"":10.0,""description"":""des""}]",
                CategoryIdList = [1, 2]
            };

            var uploadResult = new FileUploadResult
            {
                Url = "",
                PublicId = "",
                Error = "update image failed"
            };

            _cloudinaryMock.Setup(x => x.AddImageAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync(uploadResult);


            // Act &&  Assert
            await Assert.ThrowsAsync<UploadFileFailedException>(() => _sut.CreateAsync(request));
        }

        [Fact]
        public async void CreateAsync_ShouldThrowSaveChangesFailedException_WhenSaveChangesFailed()
        {
            // Arrange
            var request = new ItemCreateRequest
            {
                Name = "item",
                Description = "item",
                Slug = "item",
                ImgFile = new Mock<IFormFile>().Object,
                SizeCreateRequestJson = @"[{""name"":""S"",""price"":10.0,""newPrice"":8.0, ""description"": ""des""},{""name"":""M"",""price"":12.0,""newPrice"":10.0,""description"":""des""}]",
                CategoryIdList = [1, 2]
            };

            var uploadResult = new FileUploadResult
            {
                Url = "https://static.nutscdn.com/vimg/0-0/71ee65b75262619d4fc6a431bcd20802.jpg",
                PublicId = "public_id",
                Error = null
            };

            var sizeCreateRequests = JsonConvert.DeserializeObject<List<SizeCreateRequest>>(request.SizeCreateRequestJson);
            var sizes = sizeCreateRequests?.Select(sizeRequest => new Size
            {
                Name = sizeRequest.Name,
                Description = sizeRequest.Description,
                Price = sizeRequest.Price,
                NewPrice = sizeRequest.NewPrice
            }).ToList();

            var categories = new List<Category>
            {
                new Category {Id=1, Name = "category", Description = "category", Slug = "category" },
                new Category {Id=2, Name = "category2", Description = "category2", Slug = "category2" },
            };

            var entity = new Item
            {
                Id = 1,
                Name = request.Name,
                Description = request.Description,
                Slug = request.Slug,
                ItemCategories = categories.Select(x => new ItemCategory { Category = x, ItemId = 1, CategoryId = x.Id }).ToList(),
                PublicId = uploadResult.PublicId,
                ImgUrl = uploadResult.Url,
                Sizes = sizes ?? []
            };

            _cloudinaryMock.Setup(x => x.AddImageAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync(uploadResult);

            _unitMock.Setup(x => x.Item.Add(entity));

            _unitMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(false);

            // Act &&  Assert
            await Assert.ThrowsAsync<SaveChangesFailedException>(() => _sut.CreateAsync(request));
        }


        [Fact]
        public async void DeleteAsync_ShouldSoftDeleteItem_WhenItemExistsAndSaveChangesSucceed()
        {
            // Arrange
            int id = 1;

            var entity = new Item
            {
                Id = 1,
                Name = "item",
                Description = "item",
                Slug = "item",
                ImgUrl = "item",
            };

            _unitMock.Setup(x => x.Item.FindAsync(It.IsAny<Expression<Func<Item, bool>>>(), true))
                .ReturnsAsync(entity);

            _unitMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(true);

            // Act
            await _sut.DeleteAsync(id);

            // Assert
            Assert.True(entity.IsDeleted);
            Assert.False(entity.IsPublished);
        }

        [Fact]
        public async void DeleteAsync_ShouldThrowItemNotFoundException_WhenItemNotFound()
        {
            // Arrange
            int id = 1;

            _unitMock.Setup(x => x.Item.FindAsync(It.IsAny<Expression<Func<Item, bool>>>(), true))
                .ReturnsAsync((Item)null);

            // Act && Assert
            await Assert.ThrowsAsync<ItemNotFoundException>(() => _sut.DeleteAsync(id));
        }

        [Fact]
        public async void DeleteAsync_ShouldThrowSaveChangesFailedException_WhenSaveChangeFailed()
        {
            // Arrange
            int id = 1;

            var entity = new Item
            {
                Id = 1,
                Name = "item",
                Description = "item",
                Slug = "item",
                ImgUrl = "item",
            };

            _unitMock.Setup(x => x.Item.FindAsync(It.IsAny<Expression<Func<Item, bool>>>(), true))
                .ReturnsAsync(entity);

            _unitMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(false);


            // Act && Assert
            await Assert.ThrowsAsync<SaveChangesFailedException>(() => _sut.DeleteAsync(id));
        }


        [Fact]
        public async void DeleteSizesAsync_ShouldRemoveSizeOfItem_WhenSaveChangesSuccess()
        {
            // Arrange
            var itemId = 1;
            var sizeIdList = new List<int>() { 1, 2 };

            var sizes = new List<Size>
            {
                new Size {Id = 1, Name = "size1", Description = "des 1", NewPrice = null, Price = 1},
                new Size {Id=2, Name = "size2", Description = "des 2", NewPrice = null, Price = 2},
                new Size {Id = 3, Name = "size3", Description = "des 3", NewPrice = null, Price = 3},
            };

            var entity = new Item
            {
                Id = 1,
                Name = "item",
                Description = "item",
                Slug = "item",
                ImgUrl = "item",
                Sizes = sizes,
            };

            _unitMock.Setup(x => x.Item.FindAsync(It.IsAny<Expression<Func<Item, bool>>>(), true))
                 .ReturnsAsync(entity);

            _unitMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(true);

            // Act
            await _sut.DeleteSizesAsync(itemId, sizeIdList);

            // Assert
            Assert.Equal(1, entity.Sizes.Count);
            Assert.DoesNotContain(entity.Sizes, x => sizeIdList.Contains(x.Id));
            Assert.Contains(entity.Sizes, x => x.Id == 3);
        }

        [Fact]
        public async void DeleteSizesAsync_ShouldThrowEmptySizeListException_WhenSizeIdListIsEmptyOrNull()
        {
            // Arrange
            var itemId = 1;
            var sizeIdList = new List<int>();

            // Act && Assert
            await Assert.ThrowsAsync<EmptySizeListException>(() => _sut.DeleteSizesAsync(itemId, sizeIdList));
        }

        [Fact]
        public async void DeleteSizesAsync_ShouldThrowItemNotFoundException_WhenItemNotFound()
        {
            // Arrange
            var itemId = 1;
            var sizeIdList = new List<int>() { 1, 2 };


            _unitMock.Setup(x => x.Item.FindAsync(It.IsAny<Expression<Func<Item, bool>>>(), true))
                 .ReturnsAsync((Item)null);

            // Act && Assert
            await Assert.ThrowsAsync<ItemNotFoundException>(() => _sut.DeleteSizesAsync(itemId, sizeIdList));
        }

        [Fact]
        public async void DeleteSizesAsync_ShouldThrowItemMustHaveAtLeastOneSizeException_WhenNoSizesLeft()
        {
            // Arrange
            var itemId = 1;
            var sizeIdList = new List<int>() { 1, 2, 3 };

            var sizes = new List<Size>
            {
                new Size {Id = 1, Name = "size1", Description = "des 1", NewPrice = null, Price = 1},
                new Size {Id=2, Name = "size2", Description = "des 2", NewPrice = null, Price = 2},
                new Size {Id = 3, Name = "size3", Description = "des 3", NewPrice = null, Price = 3},
            };

            var entity = new Item
            {
                Id = 1,
                Name = "item",
                Description = "item",
                Slug = "item",
                ImgUrl = "item",
                Sizes = sizes,
            };

            _unitMock.Setup(x => x.Item.FindAsync(It.IsAny<Expression<Func<Item, bool>>>(), true))
                 .ReturnsAsync(entity);

            _unitMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(true);

            // Act && Assert
            await Assert.ThrowsAsync<ItemMustHaveAtLeastOneSizeException>(() => _sut.DeleteSizesAsync(itemId, sizeIdList));
        }

        [Fact]
        public async void DeleteSizesAsync_ShouldSaveChangesFailedException_WhenSaveChangesFailed()
        {
            // Arrange
            var itemId = 1;
            var sizeIdList = new List<int>() { 1, 2 };

            var sizes = new List<Size>
            {
                new Size {Id = 1, Name = "size1", Description = "des 1", NewPrice = null, Price = 1},
                new Size {Id=2, Name = "size2", Description = "des 2", NewPrice = null, Price = 2},
                new Size {Id = 3, Name = "size3", Description = "des 3", NewPrice = null, Price = 3},
            };

            var entity = new Item
            {
                Id = 1,
                Name = "item",
                Description = "item",
                Slug = "item",
                ImgUrl = "item",
                Sizes = sizes,
            };

            _unitMock.Setup(x => x.Item.FindAsync(It.IsAny<Expression<Func<Item, bool>>>(), true))
                 .ReturnsAsync(entity);

            _unitMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(false);

            // Act && Assert
            await Assert.ThrowsAsync<SaveChangesFailedException>(() => _sut.DeleteSizesAsync(itemId, sizeIdList));
        }


        [Fact]
        public async void GetByIdAsync_ShouldReturnItemResponse_WhenItemExists()
        {
            // Arrange
            int id = 1;
            var entity = new Item
            {
                Id = 1,
                Name = "item",
                Description = "item",
                Slug = "item",
                ImgUrl = "item",
            };

            _unitMock.Setup(x => x.Item.FindAsync(It.IsAny<Expression<Func<Item, bool>>>(), false))
                .ReturnsAsync(entity);

            // Act
            var result = await _sut.GetByIdAsync(id);

            // Assert
            Assert.Equal(id, result.Id);
            Assert.Equal(entity.Name, result.Name);
            Assert.Equal(entity.Description, result.Description);
            Assert.Equal(entity.Slug, result.Slug);
            Assert.Equal(entity.ImgUrl, result.ImgUrl);
        }

        [Fact]
        public async void GetByIdAsync_ShouldThrowItemNotFoundException_WhenItemNotFound()
        {
            // Arrange
            int id = 1;

            _unitMock.Setup(x => x.Item.FindAsync(It.IsAny<Expression<Func<Item, bool>>>(), false))
                .ReturnsAsync((Item)null);

            // Act && Assert
            await Assert.ThrowsAsync<ItemNotFoundException>(() => _sut.GetByIdAsync(id));
        }


        [Fact]
        public async void GetPaginationAsync_ShouldReturnPaginationResponse_WhenDataExists()
        {
            // Arrange
            var request = new PaginationRequest
            {
                PageSize = 5,
                PageIndex = 1,
            };

            var sizes = new List<Size>
            {
                new Size {Id = 1, Name = "size1", Description = "des 1", NewPrice = null, Price = 1},
                new Size {Id=2, Name = "size2", Description = "des 2", NewPrice = null, Price = 2},
                new Size {Id = 3, Name = "size3", Description = "des 3", NewPrice = null, Price = 3},
            };

            var categories = new List<Category>
            {
                new Category {Id=1, Name = "category", Description = "category", Slug = "category" },
                new Category {Id=2, Name = "category2", Description = "category2", Slug = "category2" },
            };

            var items = new List<Item>()
            {
                new Item{Id = 1, Name = "item",Description = "item",Slug = "item",ImgUrl = "item", Sizes = sizes, ItemCategories = categories.Select(x => new ItemCategory { Category = x, ItemId = 1, CategoryId = x.Id }).ToList(),},
                new Item{Id = 2, Name = "item2",Description = "item2",Slug = "item2",ImgUrl = "item2",Sizes = sizes, ItemCategories = categories.Select(x => new ItemCategory { Category = x, ItemId = 1, CategoryId = x.Id }).ToList(),},
            };

            var paginationResult = new PaginationResponse<Item>(items, items.Count, request.PageIndex, request.PageSize);

            _unitMock.Setup(x => x.Item.GetPaginationAsync(request, null)).ReturnsAsync(paginationResult);

            // Act
            var result = await _sut.GetPaginationAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(paginationResult.TotalCount, result.TotalCount);
            Assert.Equal(paginationResult.PageSize, result.PageSize);
            Assert.Equal(paginationResult.CurrentPage, result.CurrentPage);
            Assert.Equal(paginationResult.PageSize, result.PageSize);

            Assert.Equal(items.Count, result.Count);
            for (int i = 0; i < items.Count; i++)
            {
                Assert.Equal(items[i].Id, result.ElementAt(i).Id);
                Assert.Equal(items[i].Name, result.ElementAt(i).Name);
                Assert.Equal(items[i].Description, result.ElementAt(i).Description);
                Assert.Equal(items[i].Slug, result.ElementAt(i).Slug);

                // Kiểm tra Sizes
                Assert.Equal(items[i].Sizes.Count, result.ElementAt(i).Sizes.Count);
                for (int j = 0; j < items[i].Sizes.Count; j++)
                {
                    Assert.Equal(items[i].Sizes[j].Id, result.ElementAt(i).Sizes.ElementAt(j).Id);
                    Assert.Equal(items[i].Sizes[j].Name, result.ElementAt(i).Sizes.ElementAt(j).Name);
                }

                // Kiểm tra ItemCategories
                Assert.Equal(items[i].ItemCategories.Count, result.ElementAt(i).Categories.Count);
                for (int j = 0; j < items[i].ItemCategories.Count; j++)
                {
                    Assert.Equal(items[i].ItemCategories[j].CategoryId, result.ElementAt(i).Categories.ElementAt(j).Id);
                }
            }
        }

        [Fact]
        public async void GetPaginationAsync_ShouldReturnEmptyPaginationResponse_WhenNoDataExists()
        {
            // Arrange
            var request = new PaginationRequest
            {
                PageSize = 5,
                PageIndex = 1,
            };

            var item = new List<Item>();

            var paginationResult = new PaginationResponse<Item>(item, item.Count, request.PageIndex, request.PageSize);

            _unitMock.Setup(x => x.Item.GetPaginationAsync(request, null)).ReturnsAsync(paginationResult);

            // Act
            var result = await _sut.GetPaginationAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            Assert.Equal(0, result.TotalCount);
            Assert.Equal(paginationResult.PageSize, result.PageSize);
            Assert.Equal(paginationResult.CurrentPage, result.CurrentPage);
            Assert.Equal(paginationResult.PageSize, result.PageSize);
        }

        [Fact]
        public async void GetPublicPaginationAsync_ShouldReturnEmptyPaginationResponse_WhenNoDataExists()
        {
            // Arrange
            var request = new PaginationRequest
            {
                PageSize = 5,
                PageIndex = 1,
            };

            var item = new List<Item>();

            var paginationResult = new PaginationResponse<Item>(item, item.Count, request.PageIndex, request.PageSize);

            _unitMock.Setup(x => x.Item.GetPaginationAsync(request, It.IsAny<Expression<Func<Item, bool>>>())).ReturnsAsync(paginationResult);

            // Act
            var result = await _sut.GetPaginationAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            Assert.Equal(0, result.TotalCount);
            Assert.Equal(paginationResult.PageSize, result.PageSize);
            Assert.Equal(paginationResult.CurrentPage, result.CurrentPage);
            Assert.Equal(paginationResult.PageSize, result.PageSize);
        }
    }
}
