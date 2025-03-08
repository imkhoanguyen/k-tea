using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Moq;
using Tea.Application.DTOs.Categories;
using Tea.Application.Services.Implements;
using Tea.Domain.Common;
using Tea.Domain.Entities;
using Tea.Domain.Exceptions;
using Tea.Domain.Repositories;

namespace Tea.Application.Unit.Tests
{
    public class CategoryServiceTests
    {
        private readonly CategoryService _sut;
        private readonly Mock<IUnitOfWork> _unitMock;
        private readonly Mock<ILogger<CategoryService>> _loggerMock;

        #region Helper
        private const string saveChangesFailExceptionString = "Failed to save changes Category to the database.";
        private string categoryNotFoundException(int id)
        {
           return $"The category with id: {id} doesn't exist in the database.";
        }
        #endregion

        public CategoryServiceTests()
        {
            _unitMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<CategoryService>>();
            _sut = new CategoryService(_unitMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task CreateChildrenAsync_ShouldReturnCategoryResponse_WhenCategoryIsCreated()
        {
            // Arrange
            var request = new CategoryCreateChildrenRequest
            {
                Name = "milk tea",
                Description = "des milk tea",
                Slug = "milk-tea",
                ParentId = 1
            };

            var parent = new Category
            {
                Id = 1,
                Name = "Food",
                Description = "Des Food",
                Slug = "food",
                Children = new List<Category>()
            };

            var children = new Category
            {
                Id = 2,
                Name = request.Name,
                Description = request.Description,
                Slug = request.Slug,
                ParentId = 1
            };

            

            _unitMock.Setup(x => x.Category.FindAsync(It.IsAny<Expression<Func<Category, bool>>>(), true))
                .ReturnsAsync(parent);

            _unitMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(true);

            _unitMock.Setup(x => x.Category.FindAsync(It.IsAny<Expression<Func<Category, bool>>>(), false))
                     .ReturnsAsync(children);

            // Act
            var result = await _sut.CreateChildrenAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(children.Id, result.Id);
            Assert.Equal(children.Name, result.Name);
            Assert.Equal(children.Description, result.Description);
            Assert.Equal(children.Slug, result.Slug);
        }

        [Fact]
        public async Task CreateChildrenAsync_ShouldThrowCategoryNotFoundException_WhenParentNotFound()
        {
            // Arrange

            var request = new CategoryCreateChildrenRequest
            {
                Name = "milk tea",
                Description = "des milk tea",
                Slug = "milk-tea",
                ParentId = 1
            };

            _unitMock.Setup(x => x.Category.FindAsync(It.IsAny<Expression<Func<Category, bool>>>(), true))
                .ReturnsAsync((Category)null);

            // Act
            var exception = await Assert.ThrowsAsync<CategoryNotFoundException>(() => _sut.CreateChildrenAsync(request));

            // Assert
            Assert.Equal(categoryNotFoundException(request.ParentId), exception.Message);
        }

        [Fact]
        public async Task CreateChildrenAsync_ShouldThrowSaveChangesFailedException_WhenSaveChangesFailed()
        {
            // Arrange
            var request = new CategoryCreateChildrenRequest
            {
                Name = "milk tea",
                Description = "des milk tea",
                Slug = "milk-tea",
                ParentId = 1
            };

            var parent = new Category
            {
                Id = 1,
                Name = "Food",
                Description = "Des Food",
                Slug = "food",
                Children = new List<Category>()
            };

            _unitMock.Setup(x => x.Category.FindAsync(It.IsAny<Expression<Func<Category, bool>>>(), true))
                .ReturnsAsync(parent);

            _unitMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(false);

            // Act
            var exception = await Assert.ThrowsAsync<SaveChangesFailedException>(() => _sut.CreateChildrenAsync(request));

            // Assert
            Assert.Equal(saveChangesFailExceptionString, exception.Message);
        }


        [Fact]
        public async Task CreateParentAsync_ShouldReturnCategoryResponse_WhenCategoryIsCreated()
        {
            // Arrange
            var request = new CategoryCreateParentRequest
            {
                Name = "milk tea",
                Description = "des milk tea",
                Slug = "milk-tea",
            };

            var category = new Category
            {
                Id = 1,
                Name = request.Name,
                Description = request.Description,
                Slug = request.Slug,
            };

            _unitMock.Setup(x => x.Category.Add(It.IsAny<Category>()));

            _unitMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(true);

            _unitMock.Setup(x => x.Category.FindAsync(It.IsAny<Expression<Func<Category, bool>>>(), false))
                     .ReturnsAsync(category);

            // Act
            var result = await _sut.CreateParentAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(category.Id, result.Id);
            Assert.Equal(category.Name, result.Name);
            Assert.Equal(category.Description, result.Description);
            Assert.Equal(category.Slug, result.Slug);
        }

        [Fact]
        public async Task CreateParentAsync_ShouldThrowSaveChangesFailedException_WhenSaveChangesFailed()
        {
            // Arrange
            var request = new CategoryCreateParentRequest
            {
                Name = "milk tea",
                Description = "des milk tea",
                Slug = "milk-tea",
            };

            _unitMock.Setup(x => x.Category.Add(It.IsAny<Category>()));

            _unitMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(false);

            // Act
            var exception = await Assert.ThrowsAsync<SaveChangesFailedException>(() => _sut.CreateParentAsync(request));

            // Assert
            Assert.Equal(saveChangesFailExceptionString, exception.Message);
        }

        [Fact]
        public async Task DeleteAsync_ShouldSoftDeleteCategory_WhenCategoryExistsAndSaveChangesSucceed()
        {
            // Arrange
            int id = 1;
            var category = new Category
            {
                Id = id,
                Name = "milk tea",
                Description = "des milk tea",
                Slug = "milk-tea"
            };
             
            _unitMock.Setup(x => x.Category.FindAsync(It.IsAny<Expression<Func<Category, bool>>>(), true))
            .ReturnsAsync(category);

            _unitMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(true);

            // Act
            await _sut.DeleteAsync(id);

            // Assert
            Assert.True(category.IsDeleted);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowCategoryNotFoundException_WhenCategoryFound()
        {
            // Arrange
            int id = 0;

            _unitMock.Setup(x => x.Category.FindAsync(It.IsAny<Expression<Func<Category, bool>>>(), true))
                .ReturnsAsync((Category)null);

            // Act
            var exception = await Assert.ThrowsAsync<CategoryNotFoundException>(() => _sut.DeleteAsync(id));

            // Assert
            Assert.Equal(categoryNotFoundException(id), exception.Message);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowSaveChangesFailedException_WhenSaveChangesFailed()
        {
            // Arrange
            int id = 1;

            var category = new Category
            {
                Id = id,
                Name = "milk tea",
                Description = "des milk tea",
                Slug = "milk-tea"
            };

            _unitMock.Setup(x => x.Category.FindAsync(It.IsAny<Expression<Func<Category, bool>>>(), true))
            .ReturnsAsync(category);

            _unitMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(false);

            // Act
            var exception = await Assert.ThrowsAsync<SaveChangesFailedException>(() => _sut.DeleteAsync(id));

            // Assert
            Assert.Equal(saveChangesFailExceptionString, exception.Message);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCategoryResponse_WhenCategoryExists()
        {
            // Arrange
            int id = 1;
            var category = new Category
            {
                Id = id,
                Name = "milk tea",
                Description = "des milk tea",
                Slug = "milk-tea"
            };

            _unitMock.Setup(x => x.Category.FindAsync(It.IsAny<Expression<Func<Category, bool>>>(), false))
            .ReturnsAsync(category);

            // Act
            var result = await _sut.GetByIdAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(category.Id, result.Id);
            Assert.Equal(category.Name, result.Name);
            Assert.Equal(category.Description, result.Description);
            Assert.Equal(category.Slug, result.Slug);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowCategoryNotFoundException_WhenCategoryNotFound()
        {
            // Arrange
            int id = 0;

            _unitMock.Setup(x => x.Category.FindAsync(It.IsAny<Expression<Func<Category, bool>>>(), false))
            .ReturnsAsync((Category)null);

            // Act
            var exception = await Assert.ThrowsAsync<CategoryNotFoundException>(() => _sut.GetByIdAsync(id));

            // Assert
            Assert.Equal(categoryNotFoundException(id), exception.Message);
        }

        [Fact]
        public async Task GetPaginationAsync_ShouldReturnPaginationResponse_WhenDataExists()
        {
            // Arrange
            var request = new PaginationRequest
            {
                PageSize = 5,
                PageIndex = 1,
            };

            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category 1", Description = "Description 1", Slug = "category-1" },
                new Category { Id = 2, Name = "Category 2", Description = "Description 2", Slug = "category-2" }
            };

            var paginationResult = new PaginationResponse<Category>(request.PageIndex, request.PageSize, categories.Count, categories);


            _unitMock.Setup(x => x.Category.GetPaginationAsync(request)).ReturnsAsync(paginationResult);


            // Act
            var result = await _sut.GetPaginationAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(paginationResult.Count, result.Count);
            Assert.Equal(paginationResult.PageSize, result.PageSize);
            Assert.Equal(paginationResult.PageIndex, result.PageIndex);

            Assert.Equal(categories.Count, result.Count);
            for(int i = 0; i < categories.Count; i++)
            {
                Assert.Equal(categories[i].Id, result.Data.ElementAt(i).Id);
                Assert.Equal(categories[i].Name, result.Data.ElementAt(i).Name);
                Assert.Equal(categories[i].Description, result.Data.ElementAt(i).Description);
                Assert.Equal(categories[i].Slug, result.Data.ElementAt(i).Slug);
            }
        }

        [Fact]
        public async Task GetPaginationAsync_ShouldReturnEmptyList_WhenNoDataExists()
        {
            // Arrange
            var request = new PaginationRequest
            {
                PageSize = 5,
                PageIndex = 1,
            };

            var emptyCategories = new List<Category>();


            var paginationResult = new PaginationResponse<Category>(request.PageIndex, request.PageSize, emptyCategories.Count, emptyCategories);

            _unitMock.Setup(x => x.Category.GetPaginationAsync(request)).ReturnsAsync(paginationResult);


            // Act
            var result = await _sut.GetPaginationAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.Count);
            Assert.Equal(paginationResult.PageSize, result.PageSize);
            Assert.Equal(paginationResult.PageIndex, result.PageIndex);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnCategoryResponse_WhenCategoryExistsAndSaveChangesSucceed()
        {
            // Arrange
            int id = 1;

            var request = new CategoryUpdateRequest
            {
                Id = id,
                Name = "updated name",
                Description = "updated description",
                Slug = "updated-name"
            };

            var category = new Category
            {
                Id = id,
                Name = "default name",
                Description = "default des",
                Slug = "default slug",
            };

            var updatedCategory = new Category
            {
                Id = id,
                Name = request.Name,
                Description = request.Description,
                Slug = request.Slug,
            };

            _unitMock.Setup(x => x.Category.FindAsync(It.IsAny<Expression<Func<Category, bool>>>(), true))
            .ReturnsAsync(category);

            _unitMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(true);

            _unitMock.Setup(x => x.Category.FindAsync(It.IsAny<Expression<Func<Category, bool>>>(), false))
            .ReturnsAsync(updatedCategory);

            // Act
            var result = await _sut.UpdateAsync(id, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updatedCategory.Id, result.Id);
            Assert.Equal(updatedCategory.Name, result.Name);
            Assert.Equal(updatedCategory.Description, result.Description);
            Assert.Equal(updatedCategory.Slug, result.Slug);
        }


        [Fact]
        public async Task UpdateAsync_ShouldThrowCategoryNotFoundException_WhenCategoryNotFound()
        {
            // Arrange
            int id = 1;

            var request = new CategoryUpdateRequest
            {
                Id = id,
                Name = "updated name",
                Description = "updated description",
                Slug = "updated-name"
            };

            _unitMock.Setup(x => x.Category.FindAsync(It.IsAny<Expression<Func<Category, bool>>>(), true))
            .ReturnsAsync((Category)null);

            // Act
            var exception = await Assert.ThrowsAsync<CategoryNotFoundException>(() => _sut.UpdateAsync(id, request));

            // Assert
            Assert.Equal(categoryNotFoundException(id), exception.Message);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowSaveChangesFailedException_WhenSaveChangesFailed()
        {
            // Arrange
            int id = 1;

            var request = new CategoryUpdateRequest
            {
                Id = id,
                Name = "updated name",
                Description = "updated description",
                Slug = "updated-name"
            };

            var category = new Category
            {
                Id = id,
                Name = "default name",
                Description = "default des",
                Slug = "default slug",
            };

            var updatedCategory = new Category
            {
                Id = id,
                Name = request.Name,
                Description = request.Description,
                Slug = request.Slug,
            };

            _unitMock.Setup(x => x.Category.FindAsync(It.IsAny<Expression<Func<Category, bool>>>(), true))
            .ReturnsAsync(category);

            _unitMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(false);

            // Act
            var exception = await Assert.ThrowsAsync<SaveChangesFailedException>(() => _sut.UpdateAsync(id, request));

            // Assert
            Assert.Equal(saveChangesFailExceptionString, exception.Message);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowIdMismatchException_WhenIdRouteAndIdBodyNotSame()
        {
            // Arrange
            int routeId = 1;

            var request = new CategoryUpdateRequest
            {
                Id = 2,
                Name = "updated name",
                Description = "updated description",
                Slug = "updated-name"
            };

            // Act
            var exception = await Assert.ThrowsAsync<IdMismatchException>(() => _sut.UpdateAsync(routeId, request));

            // Assert
            Assert.Equal($"Id: {routeId} in the route does not match the Id: {request.Id} in the request body.",
                exception.Message);
        }
    }
}