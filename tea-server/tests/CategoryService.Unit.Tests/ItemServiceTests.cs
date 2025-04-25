using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Tea.Application.DTOs.Categories;
using Tea.Application.DTOs.Cloudinaries;
using Tea.Application.DTOs.Items;
using Tea.Application.DTOs.Sizes;
using Tea.Application.Interfaces;
using Tea.Application.Services.Implements;
using Tea.Application.Services.Interfaces;
using Tea.Domain.Common;
using Tea.Domain.Constants;
using Tea.Domain.Entities;
using Tea.Domain.Exceptions;
using Tea.Domain.Exceptions.BadRequests;
using Tea.Domain.Exceptions.NotFounds;
using Tea.Domain.Repositories;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        public async Task AddSizesAsync_ShouldReturnItemResponse_WhenSizesAddedToItem()
        {
            // Arrange
            int itemId = 1;
            var requests = new List<SizeCreateRequest>
            {
                new SizeCreateRequest { Name = "size1", Description = "des 1", Price = 1 },
                new SizeCreateRequest { Name = "size2", Description = "des 2", Price = 2 },
                new SizeCreateRequest { Name = "size3", Description = "des 3", Price = 3 },
            };

            var item = new Item
            {
                Id = itemId,
                Name = "item",
                Description = "item",
                Slug = "item",
                ImgUrl = "item",
                Sizes = new List<Size>(),
                ItemCategories = new List<ItemCategory>(),
            };

            _unitMock.Setup(x => x.Item.FindAsync(It.IsAny<Expression<Func<Item, bool>>>(), true))
                .ReturnsAsync(item);

            _unitMock.Setup(x => x.BeginTransactionAsync())
                .Returns(Task.CompletedTask);

            _unitMock.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(true);

            _unitMock.Setup(x => x.CommitTransactionAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _sut.AddSizesAsync(itemId, requests);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(itemId, result.Id);
            Assert.Equal(3, result.Sizes.Count);
        }

        [Fact]
        public async Task AddSizesAsync_ShouldThrowEmptySizeListException_WhenSizeListEmpty()
        {
            // Arrange
            int itemId = 1;
            var requests = new List<SizeCreateRequest>();

            // Act & Assert
            await Assert.ThrowsAsync<EmptySizeListException>(() => _sut.AddSizesAsync(itemId, requests));
        }

        [Fact]
        public async Task AddSizesAsync_ShouldThrowItemNotFoundException_WhenItemNotFound()
        {
            // Arrange
            int itemId = 1;
            var requests = new List<SizeCreateRequest>
            {
                new SizeCreateRequest { Name = "size1", Description = "des 1", Price = 1 }
            };

            _unitMock.Setup(x => x.Item.FindAsync(It.IsAny<Expression<Func<Item, bool>>>(), true))
                .ReturnsAsync((Item)null);

            // Act & Assert
            await Assert.ThrowsAsync<ItemNotFoundException>(() => _sut.AddSizesAsync(itemId, requests));
        }

        [Fact]
        public async Task AddSizesAsync_ShouldThrowSaveChangeFailedException_WhenSaveChangeFailed()
        {
            // Arrange
            int itemId = 1;
            var requests = new List<SizeCreateRequest>
            {
                new SizeCreateRequest { Name = "size1", Description = "des 1", Price = 1 }
            };

            var item = new Item
            {
                Id = itemId,
                Sizes = new List<Size>(),
                Name = "t",
                Slug = "t",
                ImgUrl = "t"
            };

            _unitMock.Setup(x => x.Item.FindAsync(It.IsAny<Expression<Func<Item, bool>>>(), true))
                .ReturnsAsync(item);

            _unitMock.Setup(x => x.BeginTransactionAsync())
                .Returns(Task.CompletedTask);

            _unitMock.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(false);

            _unitMock.Setup(x => x.RollbackTransactionAsync())
                .Returns(Task.CompletedTask);

            // Act & Assert
            await Assert.ThrowsAsync<SaveChangesFailedException>(() => _sut.AddSizesAsync(itemId, requests));
        }


        [Fact]
        public async Task CreateAsync_ShouldReturnItemResponse_WhenItemCreated()
        {
            // Arrange
            var request = new ItemCreateRequest
            {
                Name = "Test Item",
                Description = "Test Description",
                Slug = "test-item",
                IsPublished = true,
                IsFeatured = false,
                ImgFile = new Mock<IFormFile>().Object,
                CategoryIdList = new List<int> { 1, 2 }, // Assuming category IDs are integers
                SizeCreateRequestJson = JsonConvert.SerializeObject(new List<SizeCreateRequest>
                {
                    new SizeCreateRequest
                    {
                        Name = "Small",
                        Description = "Small size",
                        Price = 10.99m,
                        NewPrice = 9.99m
                    },
                    new SizeCreateRequest
                    {
                        Name = "Large",
                        Description = "Large size",
                        Price = 15.99m,
                        NewPrice = 14.99m
                    }
                })
            };

            var uploadResult = new FileUploadResult 
            {
                Url = "https://static.nutscdn.com/vimg/0-0/71ee65b75262619d4fc6a431bcd20802.jpg",
                PublicId = "test_public_id",
                Error = null
            };

            var savedItem = new Item
            {
                Id = 1,
                Name = request.Name,
                Description = request.Description,
                Slug = request.Slug,
                IsPublished = request.IsPublished,
                ImgUrl = uploadResult.Url.ToString(),
                PublicId = uploadResult.PublicId,
                IsFeatured = request.IsFeatured,
                ItemCategories = new List<ItemCategory>
                {
                    new ItemCategory (1,1),
                    new ItemCategory (1,2)
                },
                Sizes = new List<Size>
                {
                    new Size { Name = "Small", Description = "Small size", Price = 10.99m, NewPrice = 9.99m },
                    new Size { Name = "Large", Description = "Large size", Price = 15.99m, NewPrice = 14.99m }
                }
            };

            _cloudinaryMock.Setup(x => x.AddImageAsync(It.IsAny<IFormFile>()))
               .ReturnsAsync(uploadResult);

            _unitMock.Setup(x => x.BeginTransactionAsync())
                .Returns(Task.CompletedTask);

            _unitMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(true);

            _unitMock.Setup(x => x.CommitTransactionAsync())
               .Returns(Task.CompletedTask);

            _unitMock.Setup(x => x.Item.FindAsync(It.IsAny<Expression<Func<Item, bool>>>(), false))
                .ReturnsAsync(savedItem);

            // Act
            var result = await _sut.CreateAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.Name, result.Name);
            Assert.Equal(request.Description, result.Description);
            Assert.Equal(request.Slug, result.Slug);
            Assert.Equal(uploadResult.Url, result.ImgUrl);
            Assert.Equal(2, savedItem.ItemCategories.Count);
            Assert.Equal(2, savedItem.Sizes.Count);

        }



        [Fact]
        public async Task CreateAsync_ShouldThrowUploadFileFailedException_WhenUploadImageFailed()
        {
            // Arrange
            var request = new ItemCreateRequest
            {
                Name = "item",
                Description = "item",
                Slug = "item",
                ImgFile = new Mock<IFormFile>().Object,
                CategoryIdList = new List<int> { 1, 2 }, // Assuming category IDs are integers
                SizeCreateRequestJson = JsonConvert.SerializeObject(new List<SizeCreateRequest>
                {
                    new SizeCreateRequest
                    {
                        Name = "Small",
                        Description = "Small size",
                        Price = 10.99m,
                        NewPrice = 9.99m
                    },
                    new SizeCreateRequest
                    {
                        Name = "Large",
                        Description = "Large size",
                        Price = 15.99m,
                        NewPrice = 14.99m
                    }
                })
            };

            var uploadResult = new FileUploadResult 
            {
                Error = "upload image failed"
            };

            _cloudinaryMock.Setup(x => x.AddImageAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync(uploadResult);

            // Act & Assert
            await Assert.ThrowsAsync<UploadFileFailedException>(() => _sut.CreateAsync(request));
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowSaveChangesFailedException_WhenSaveChangesFailed()
        {
            // Arrange
            var request = new ItemCreateRequest
            {
                Name = "item",
                Description = "item",
                Slug = "item",
                ImgFile = new Mock<IFormFile>().Object,
                SizeCreateRequestJson = JsonConvert.SerializeObject(new List<SizeCreateRequest>
                {
                    new SizeCreateRequest
                    {
                        Name = "Small",
                        Description = "Small size",
                        Price = 10.99m,
                        NewPrice = 9.99m
                    },
                    new SizeCreateRequest
                    {
                        Name = "Large",
                        Description = "Large size",
                        Price = 15.99m,
                        NewPrice = 14.99m
                    }
                })
            };

            var uploadResult = new FileUploadResult  
            {
                Url = "https://static.nutscdn.com/vimg/0-0/71ee65b75262619d4fc6a431bcd20802.jpg",
                PublicId = "public_id",
                Error = null
            };

            var savedItem = new Item
            {
                Id = 1,
                Name = request.Name,
                Description = request.Description,
                Slug = request.Slug,
                IsPublished = request.IsPublished,
                ImgUrl = uploadResult.Url.ToString(),
                PublicId = uploadResult.PublicId,
                IsFeatured = request.IsFeatured,
                ItemCategories = new List<ItemCategory>
                {
                    new ItemCategory (1,1),
                    new ItemCategory (1,2)
                },
                Sizes = new List<Size>
                {
                    new Size { Name = "Small", Description = "Small size", Price = 10.99m, NewPrice = 9.99m },
                    new Size { Name = "Large", Description = "Large size", Price = 15.99m, NewPrice = 14.99m }
                }
            };

            var resultDeleteFile = new FileDeleteResult { Result = "deleted successfully", Error = null };


            _cloudinaryMock.Setup(x => x.AddImageAsync(It.IsAny<IFormFile>()))
               .ReturnsAsync(uploadResult);

            _unitMock.Setup(x => x.BeginTransactionAsync())
                .Returns(Task.CompletedTask);

            _unitMock.Setup(x => x.Item.Add(savedItem));

            _unitMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(false);

            _unitMock.Setup(x => x.RollbackTransactionAsync())
                .Returns(Task.CompletedTask);

            _cloudinaryMock.Setup(x => x.DeleteFileAsync(uploadResult.PublicId))
                .Returns(Task.FromResult(resultDeleteFile));

            // Act & Assert
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
            var request = new ItemPaginationRequest
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
                    new Item{Id = 1, Name = "item",Description = "item",Slug = "item",ImgUrl = "item", Sizes = sizes, ItemCategories = categories.Select(x => new ItemCategory(x,  1,x.Id)).ToList() },
                    new Item{Id = 2, Name = "item2",Description = "item2",Slug = "item2",ImgUrl = "item2",Sizes = sizes, ItemCategories = categories.Select(x => new ItemCategory(x,  1,x.Id)).ToList()},
                };

            var paginationResult = new PaginationResponse<Item>(request.PageIndex, request.PageSize, items.Count, items);

            _unitMock.Setup(x => x.Item.GetPaginationAsync(request, null)).ReturnsAsync(paginationResult);

            // Act
            var result = await _sut.GetPaginationAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(paginationResult.Count, result.Count);
            Assert.Equal(paginationResult.PageSize, result.PageSize);
            Assert.Equal(paginationResult.PageIndex, result.PageIndex);

            Assert.Equal(items.Count, result.Count);
            for (int i = 0; i < items.Count; i++)
            {
                Assert.Equal(items[i].Id, result.Data.ElementAt(i).Id);
                Assert.Equal(items[i].Name, result.Data.ElementAt(i).Name);
                Assert.Equal(items[i].Description, result.Data.ElementAt(i).Description);
                Assert.Equal(items[i].Slug, result.Data.ElementAt(i).Slug);

                // Kiểm tra Sizes
                Assert.Equal(items[i].Sizes.Count, result.Data.ElementAt(i).Sizes.Count);
                for (int j = 0; j < items[i].Sizes.Count; j++)
                {
                    Assert.Equal(items[i].Sizes[j].Id, result.Data.ElementAt(i).Sizes.ElementAt(j).Id);
                    Assert.Equal(items[i].Sizes[j].Name, result.Data.ElementAt(i).Sizes.ElementAt(j).Name);
                }

                // Kiểm tra ItemCategories
                Assert.Equal(items[i].ItemCategories.Count, result.Data.ElementAt(i).Categories.Count);
                for (int j = 0; j < items[i].ItemCategories.Count; j++)
                {
                    Assert.Equal(items[i].ItemCategories[j].CategoryId, result.Data.ElementAt(i).Categories.ElementAt(j).Id);
                }
            }
        }

        [Fact]
        public async void GetPaginationAsync_ShouldReturnEmptyPaginationResponse_WhenNoDataExists()
        {
            // Arrange
            var request = new ItemPaginationRequest
            {
                PageSize = 5,
                PageIndex = 1,
            };

            var item = new List<Item>();

            var paginationResult = new PaginationResponse<Item>(request.PageIndex, request.PageSize, item.Count, item);

            _unitMock.Setup(x => x.Item.GetPaginationAsync(request, null)).ReturnsAsync(paginationResult);

            // Act
            var result = await _sut.GetPaginationAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Data);
            Assert.Equal(0, result.Count);
            Assert.Equal(paginationResult.PageSize, result.PageSize);
            Assert.Equal(paginationResult.PageIndex, result.PageIndex);
        }

        [Fact]
        public async void GetPublicPaginationAsync_ShouldReturnEmptyPaginationResponse_WhenNoDataExists()
        {
            // Arrange
            var request = new ItemPaginationRequest
            {
                PageSize = 5,
                PageIndex = 1,
            };

            var item = new List<Item>();

            var paginationResult = new PaginationResponse<Item>(request.PageIndex, request.PageSize, item.Count, item);

            _unitMock.Setup(x => x.Item.GetPaginationAsync(request, It.IsAny<Expression<Func<Item, bool>>>())).ReturnsAsync(paginationResult);

            // Act
            var result = await _sut.GetPublicPaginationAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Data);
            Assert.Equal(0, result.Count);
            Assert.Equal(paginationResult.PageSize, result.PageSize);
            Assert.Equal(paginationResult.PageIndex, result.PageIndex);
        }

        [Fact]
        public async void GetPublicPaginationAsync_ShouldReturnOnlyPublishedItem_WhenDataExists()
        {
            // Arrange
            var request = new ItemPaginationRequest
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

            var expect = new List<Item>()
                {
                    new Item{Id = 1, Name = "item",Description = "item",Slug = "item",ImgUrl = "item", IsPublished = true, Sizes = sizes, ItemCategories = categories.Select(x => new ItemCategory(x,  1,x.Id)).ToList()},
                    new Item{Id = 2, Name = "item2",Description = "item2",Slug = "item2",ImgUrl = "item2", IsPublished = true, Sizes = sizes, ItemCategories = categories.Select(x => new ItemCategory(x,  1,x.Id)).ToList()},
                };

            var paginationResult = new PaginationResponse<Item>(request.PageIndex, request.PageSize, expect.Count, expect);

            _unitMock.Setup(x => x.Item.GetPaginationAsync(request, x => x.IsPublished)).ReturnsAsync(paginationResult);

            // Act
            var result = await _sut.GetPublicPaginationAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(paginationResult.Count, result.Count);
            Assert.Equal(paginationResult.PageSize, result.PageSize);
            Assert.Equal(paginationResult.PageIndex, result.PageIndex);

            Assert.Equal(expect.Count, result.Count);
            for (int i = 0; i < expect.Count; i++)
            {
                Assert.True(result.Data.ElementAt(i).IsPublished);
                Assert.Equal(expect[i].Id, result.Data.ElementAt(i).Id);
                Assert.Equal(expect[i].Name, result.Data.ElementAt(i).Name);
                Assert.Equal(expect[i].Description, result.Data.ElementAt(i).Description);
                Assert.Equal(expect[i].Slug, result.Data.ElementAt(i).Slug);

                // Kiểm tra Sizes
                Assert.Equal(expect[i].Sizes.Count, result.Data.ElementAt(i).Sizes.Count);
                for (int j = 0; j < expect[i].Sizes.Count; j++)
                {
                    Assert.Equal(expect[i].Sizes[j].Id, result.Data.ElementAt(i).Sizes.ElementAt(j).Id);
                    Assert.Equal(expect[i].Sizes[j].Name, result.Data.ElementAt(i).Sizes.ElementAt(j).Name);
                }

                // Kiểm tra ItemCategories
                Assert.Equal(expect[i].ItemCategories.Count, result.Data.ElementAt(i).Categories.Count);
                for (int j = 0; j < expect[i].ItemCategories.Count; j++)
                {
                    Assert.Equal(expect[i].ItemCategories[j].CategoryId, result.Data.ElementAt(i).Categories.ElementAt(j).Id);
                }
            }
        }


        [Fact]
        public async Task UpdateImageAsync_ShouldReturnImgUrl_WhenUploadImageSucceeds()
        {
            // Arrange
            var itemId = 1;
            var imgFile = new Mock<IFormFile>().Object;
            var entity = new Item
            {
                Id = itemId,
                Name = "item",
                PublicId = "old_public_id",
                ImgUrl = "old_image_url",
                Slug = "t"
            };

            var uploadResult = new FileUploadResult
            {
                Url = "https://example.com/new_image.jpg",
                PublicId = "new_public_id",
                Error = null
            };

            var deleteResult = new FileDeleteResult
            {
                Result = "ok",
                Error = null
            };

            _unitMock.Setup(x => x.Item.FindAsync(It.IsAny<Expression<Func<Item, bool>>>(), true))
                .ReturnsAsync(entity);

            _cloudinaryMock.Setup(x => x.AddImageAsync(imgFile))
                .ReturnsAsync(uploadResult);

            _cloudinaryMock.Setup(x => x.DeleteFileAsync(entity.PublicId))
                .ReturnsAsync(deleteResult);

            _unitMock.Setup(x => x.BeginTransactionAsync())
                .Returns(Task.CompletedTask);

            _unitMock.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(true);

            _unitMock.Setup(x => x.CommitTransactionAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _sut.UpdateImageAsync(itemId, imgFile);

            // Assert
            Assert.Equal(uploadResult.Url, result);
        }

        [Fact]
        public async Task UpdateImageAsync_ShouldThrowItemNotFoundException_WhenItemDoesNotExist()
        {
            // Arrange
            var itemId = 1;
            var imgFile = new Mock<IFormFile>().Object;

            _unitMock.Setup(x => x.Item.FindAsync(It.IsAny<Expression<Func<Item, bool>>>(), true))
                .ReturnsAsync((Item)null);

            // Act & Assert
            await Assert.ThrowsAsync<ItemNotFoundException>(() => _sut.UpdateImageAsync(itemId, imgFile));
        }

        [Fact]
        public async Task UpdateImageAsync_ShouldThrowUploadFileFailedException_WhenUploadImageFails()
        {
            // Arrange
            var itemId = 1;
            var imgFile = new Mock<IFormFile>().Object;
            var entity = new Item { Id = itemId, PublicId = "old_public_id", ImgUrl = "t", Name = "t", Slug = "T" };

            var uploadResult = new FileUploadResult
            {
                Error = "Upload failed" 
            };

            _unitMock.Setup(x => x.Item.FindAsync(It.IsAny<Expression<Func<Item, bool>>>(), true))
                .ReturnsAsync(entity);

            _cloudinaryMock.Setup(x => x.AddImageAsync(imgFile))
                .ReturnsAsync(uploadResult);

            // Act & Assert
            await Assert.ThrowsAsync<UploadFileFailedException>(() => _sut.UpdateImageAsync(itemId, imgFile));
        }

        [Fact]
        public async Task UpdateImageAsync_ShouldThrowSaveChangesFailedException_WhenSaveChangesFails()
        {
            // Arrange
            var itemId = 1;
            var imgFile = new Mock<IFormFile>().Object;
            var entity = new Item { Id = itemId, PublicId = "old_public_id", ImgUrl = "t", Name = "t", Slug = "T" };

            var uploadResult = new FileUploadResult
            {
                Url = "https://example.com/new_image.jpg",
                PublicId = "new_public_id",
                Error = null
            };

            var deleteResult = new FileDeleteResult { Result = "ok" };

            _unitMock.Setup(x => x.Item.FindAsync(It.IsAny<Expression<Func<Item, bool>>>(), true))
                .ReturnsAsync(entity);

            _cloudinaryMock.Setup(x => x.AddImageAsync(imgFile))
                .ReturnsAsync(uploadResult);

            _cloudinaryMock.Setup(x => x.DeleteFileAsync(entity.PublicId))
                .ReturnsAsync(deleteResult);

            _unitMock.Setup(x => x.BeginTransactionAsync())
                .Returns(Task.CompletedTask);

            _unitMock.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(false);

            _unitMock.Setup(x => x.RollbackTransactionAsync())
                .Returns(Task.CompletedTask);

            _cloudinaryMock.Setup(x => x.DeleteFileAsync(uploadResult.PublicId))
                .ReturnsAsync(deleteResult);

            // Act & Assert
            await Assert.ThrowsAsync<SaveChangesFailedException>(
                () => _sut.UpdateImageAsync(itemId, imgFile));
        }


        [Fact]
        public async Task UpdateItemAsync_ShouldReturnItemResponse_WhenSaveChangesSuccess()
        {
            // Arrange
            var id = 1;
            var request = new ItemUpdateRequest
            {
                Id = id,
                Name = "updated name",
                Description = "updated description",
                Slug = "updated-slug",
                IsFeatured = true,
                IsPublished = false,
                CategoryIdList = new List<int> { 1, 3 } // Remove category 2, add category 3
            };

            var existingItem = new Item
            {
                Id = id,
                Name = "original name",
                Description = "original description",
                Slug = "original-slug",
                ImgUrl = "t",
                IsFeatured = false,
                IsPublished = true,
                ItemCategories = new List<ItemCategory>
                {
                    new ItemCategory(id,1),
                    new ItemCategory(id,2)
                }
            };

            var updatedItem = new Item
            {
                Id = id,
                Name = request.Name,
                Description = request.Description,
                Slug = request.Slug,
                IsFeatured = request.IsFeatured,
                IsPublished = request.IsPublished,
                ImgUrl = "t",
                ItemCategories = request.CategoryIdList
                    .Select(c => new ItemCategory(id, c))
                    .ToList()
            };

            _unitMock.Setup(x => x.BeginTransactionAsync())
                .Returns(Task.CompletedTask);

            _unitMock.Setup(x => x.Item.FindAsync(It.IsAny<Expression<Func<Item, bool>>>(), true))
                .ReturnsAsync(existingItem);

            _unitMock.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(true);

            _unitMock.Setup(x => x.CommitTransactionAsync())
                .Returns(Task.CompletedTask);

            _unitMock.Setup(x => x.Item.FindAsync(It.IsAny<Expression<Func<Item, bool>>>(), false))
                .ReturnsAsync(updatedItem);

            // Act
            var result = await _sut.UpdateItemAsync(id, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal(request.Name, result.Name);
            Assert.Equal(request.Description, result.Description);
            Assert.Equal(request.Slug, result.Slug);
            Assert.Equal(request.IsFeatured, result.IsFeatured);
            Assert.Equal(request.IsPublished, result.IsPublished);
            Assert.Equal(2, result.Categories.Count);
        }

        [Fact]
        public async Task UpdateItemAsync_ShouldThrowIdMismatchException_WhenIdRouteAndIdBodyNotSame()
        {
            // Arrange
            var routeId = 1;
            var request = new ItemUpdateRequest { Id = 2 };

            // Act & Assert
            await Assert.ThrowsAsync<IdMismatchException>(() => _sut.UpdateItemAsync(routeId, request));
        }

        [Fact]
        public async Task UpdateItemAsync_ShouldThrowItemMustHaveAtLeastOneCategoryException_WhenCategoryIdListEmpty()
        {
            // Arrange
            var id = 1;
            var request = new ItemUpdateRequest
            {
                Id = id,
                CategoryIdList = new List<int>()
            };

            // Act & Assert
            await Assert.ThrowsAsync<ItemMustHaveAtLeastOneCategoryException>(
                () => _sut.UpdateItemAsync(id, request));
        }

        [Fact]
        public async Task UpdateItemAsync_ShouldThrowItemNotFoundException_WhenItemNotFound()
        {
            // Arrange
            var id = 1;
            var request = new ItemUpdateRequest
            {
                Id = id,
                CategoryIdList = new List<int> { 1 }
            };

            _unitMock.Setup(x => x.Item.FindAsync(It.IsAny<Expression<Func<Item, bool>>>(), true))
                .ReturnsAsync((Item)null);

            // Act & Assert
            await Assert.ThrowsAsync<ItemNotFoundException>(() => _sut.UpdateItemAsync(id, request));
        }

        [Fact]
        public async Task UpdateItemAsync_ShouldThrowSaveChangesFailedException_WhenSaveChangesFail()
        {
            // Arrange
            var id = 1;
            var request = new ItemUpdateRequest
            {
                Id = id,
                CategoryIdList = new List<int> { 1 }
            };

            var existingItem = new Item
            {
                Id = id,
                ItemCategories = new List<ItemCategory>(),
                Name = "T",
                ImgUrl =  "T",
                Slug = "T",
            };

            _unitMock.Setup(x => x.BeginTransactionAsync())
                .Returns(Task.CompletedTask);

            _unitMock.Setup(x => x.Item.FindAsync(It.IsAny<Expression<Func<Item, bool>>>(), true))
                .ReturnsAsync(existingItem);

            _unitMock.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(false);

            _unitMock.Setup(x => x.RollbackTransactionAsync())
                .Returns(Task.CompletedTask);

            // Act & Assert
            await Assert.ThrowsAsync<SaveChangesFailedException>(() => _sut.UpdateItemAsync(id, request));
        }


        [Fact]
        public async Task UpdateSizeAsync_ShouldReturnItemResponse_WhenSaveChangeSuccess()
        {
            // Arrange
            var itemId = 1;
            var sizeId = 1;
            var request = new SizeUpdateRequest
            {
                Id = sizeId,
                ItemId = itemId,
                Name = "Updated Size",
                Description = "Updated Description",
                Price = 200,
                NewPrice = 150
            };

            var originalSize = new Size
            {
                Id = sizeId,
                Name = "Original Size",
                Description = "Original Description",
                Price = 100,
                NewPrice = null
            };

            var item = new Item
            {
                Id = itemId,
                Name = "Test Item",
                Sizes = new List<Size> { originalSize },
                ImgUrl = "t",
                Slug = "t"
            };

            _unitMock.Setup(x => x.Item.FindAsync(It.IsAny<Expression<Func<Item, bool>>>(), true))
                .ReturnsAsync(item);

            _unitMock.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(true);

            // Act
            var result = await _sut.UpdateSizeAsync(itemId, request);

            // Assert
            Assert.NotNull(result);

            // Verify size was updated correctly
            var updatedSize = item.Sizes.First();
            Assert.Equal(request.Name, updatedSize.Name);
            Assert.Equal(request.Description, updatedSize.Description);
            Assert.Equal(request.Price, updatedSize.Price);
            Assert.Equal(request.NewPrice, updatedSize.NewPrice);
        }

        [Fact]
        public async Task UpdateSizeAsync_ShouldThrowIdMismatchException_WhenIdRouteAndIdBodyNotSame()
        {
            // Arrange
            var routeId = 1;
            var request = new SizeUpdateRequest
            {
                ItemId = 2, // Different from routeId
                Id = 1
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<IdMismatchException>(
                () => _sut.UpdateSizeAsync(routeId, request));

            Assert.Equal("Vui lòng chọn lại sản phẩm của size cần cập nhật.", exception.Message);

            // Verify logging
            _loggerMock.Verify(x => x.LogWarning(
                It.Is<string>(s => s.Contains($"Route ID: {routeId}") && s.Contains($"Body ID: {request.ItemId}"))),
                Times.Once);
        }

        [Fact]
        public async Task UpdateSizeAsync_ShouldThrowItemNotFoundException_WhenItemNotFound()
        {
            // Arrange
            var itemId = 1;
            var request = new SizeUpdateRequest
            {
                ItemId = itemId,
                Id = 1
            };

            _unitMock.Setup(x => x.Item.FindAsync(It.IsAny<Expression<Func<Item, bool>>>(), true))
                .ReturnsAsync((Item)null);

            // Act & Assert
            await Assert.ThrowsAsync<ItemNotFoundException>(
                () => _sut.UpdateSizeAsync(itemId, request));
        }

        [Fact]
        public async Task UpdateSizeAsync_ShouldThrowSizeNotFoundException_WhenSizeNotFoundInItem()
        {
            // Arrange
            var itemId = 1;
            var sizeId = 99; // Non-existent size
            var request = new SizeUpdateRequest
            {
                ItemId = itemId,
                Id = sizeId
            };

            var item = new Item
            {
                Id = itemId,
                Name = "t",
                Slug = "T",
                ImgUrl = "t",
                Sizes = new List<Size>
                {
                    new Size { Id = 1, Name = "t" } // Different from requested sizeId
                }
            };

            _unitMock.Setup(x => x.Item.FindAsync(It.IsAny<Expression<Func<Item, bool>>>(), true))
                .ReturnsAsync(item);

            // Act & Assert
            await Assert.ThrowsAsync<SizeNotFoundException>(
                () => _sut.UpdateSizeAsync(itemId, request));
        }

        [Fact]
        public async Task UpdateSizeAsync_ShouldThrowSaveChangesFailedException_WhenSaveChangeFail()
        {
            // Arrange
            var itemId = 1;
            var sizeId = 1;
            var request = new SizeUpdateRequest
            {
                ItemId = itemId,
                Id = sizeId
            };

            var item = new Item
            {
                Id = itemId,
                Name = "t",
                ImgUrl = "t",
                Slug = "t",
                Sizes = new List<Size>
                {
                    new Size { Id = sizeId, Name = "t" }
                }
            };

            _unitMock.Setup(x => x.Item.FindAsync(It.IsAny<Expression<Func<Item, bool>>>(), true))
                .ReturnsAsync(item);

            _unitMock.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<SaveChangesFailedException>(() => _sut.UpdateSizeAsync(itemId, request));
        }
    }
}
