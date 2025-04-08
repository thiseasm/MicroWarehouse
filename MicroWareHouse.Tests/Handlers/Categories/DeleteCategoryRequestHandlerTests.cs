using FluentAssertions;
using Microsoft.Extensions.Logging;
using MicroWarehouse.Core.Abstractions.Models.Requests.Categories;
using MicroWarehouse.Core.Handlers.Categories;
using MicroWarehouse.Infrastructure.Abstractions.Interfaces;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MicroWarehouse.Tests.Handlers.Categories
{
    public class DeleteCategoryRequestHandlerTests
    {
        private readonly ILogger<DeleteCategoryRequestHandler> _logger;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly DeleteCategoryRequestHandler _handler;

        public DeleteCategoryRequestHandlerTests()
        {
            _logger = Substitute.For<ILogger<DeleteCategoryRequestHandler>>();
            _categoryRepository = Substitute.For<ICategoryRepository>();
            _productRepository = Substitute.For<IProductRepository>();
            _handler = new DeleteCategoryRequestHandler(_logger, _categoryRepository, _productRepository);
        }

        [Fact]
        public async Task Handle_WhenCategoryDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var request = new DeleteCategoryRequest
            {
                CategoryId = new Random().Next(1,1000)
            };
            _categoryRepository.CategoryExistsAsync(request.CategoryId, Arg.Any<CancellationToken>()).Returns(false);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Handle_WhenCategoryHasAssociatedProducts_ShouldReturnConflict()
        {
            // Arrange
            var request = new DeleteCategoryRequest
            {
                CategoryId = new Random().Next(1, 1000)
            };
            _categoryRepository.CategoryExistsAsync(request.CategoryId, Arg.Any<CancellationToken>()).Returns(true);
            _productRepository.ProductsWithCategoryExistAsync(request.CategoryId, Arg.Any<CancellationToken>()).Returns(true);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task Handle_WhenCategoryIsDeletedSuccessfully_ShouldReturnOk()
        {
            // Arrange
            var request = new DeleteCategoryRequest
            {
                CategoryId = new Random().Next(1, 1000)
            };

            _categoryRepository.CategoryExistsAsync(request.CategoryId, Arg.Any<CancellationToken>()).Returns(true);
            _productRepository.ProductsWithCategoryExistAsync(request.CategoryId, Arg.Any<CancellationToken>()).Returns(false);
            _categoryRepository.RemoveAsync(request.CategoryId, Arg.Any<CancellationToken>()).Returns(true);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task Handle_WhenCategoryDeletionFails_ShouldReturnInternalError()
        {
            // Arrange
            var request = new DeleteCategoryRequest
            {
                CategoryId = new Random().Next(1, 1000)
            };

            _categoryRepository.CategoryExistsAsync(request.CategoryId, Arg.Any<CancellationToken>()).Returns(true);
            _productRepository.ProductsWithCategoryExistAsync(request.CategoryId, Arg.Any<CancellationToken>()).Returns(false);
            _categoryRepository.RemoveAsync(request.CategoryId, Arg.Any<CancellationToken>()).Returns(false);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task Handle_WhenExceptionIsThrown_ShouldReturnInternalError()
        {
            // Arrange
            var request = new DeleteCategoryRequest
            {
                CategoryId = new Random().Next(1, 1000)
            };
            _categoryRepository.CategoryExistsAsync(request.CategoryId, Arg.Any<CancellationToken>()).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.InternalServerError);
            result.Error.Should().NotBeNull();
            result.Error.Message.Should().Contain("Database error");
        }
    }
}
