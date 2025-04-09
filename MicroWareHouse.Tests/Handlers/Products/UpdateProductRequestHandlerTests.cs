using FluentAssertions;
using Microsoft.Extensions.Logging;
using MicroWarehouse.Core.Abstractions.Models.Requests.Products;
using MicroWarehouse.Core.Handlers.Products;
using MicroWarehouse.Infrastructure.Abstractions.DTOs;
using MicroWarehouse.Infrastructure.Abstractions.Interfaces;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MicroWareHouse.Tests.Handlers.Products
{
    public class UpdateProductRequestHandlerTests
    {
        private readonly ILogger<UpdateProductRequestHandler> _logger;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly UpdateProductRequestHandler _handler;

        public UpdateProductRequestHandlerTests()
        {
            _logger = Substitute.For<ILogger<UpdateProductRequestHandler>>();
            _productRepository = Substitute.For<IProductRepository>();
            _categoryRepository = Substitute.For<ICategoryRepository>();
            _handler = new UpdateProductRequestHandler(_logger, _productRepository, _categoryRepository);
        }

        [Fact]
        public async Task Handle_WhenProductDoesNotExist_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new UpdateProductRequest
            {
                Id = new Random().Next(1, 1000), 
                Name = "Updated Product", 
                CategoryId = new Random().Next(1, 1000),
            };

            _categoryRepository.CategoryExistsAsync(request.CategoryId, Arg.Any<CancellationToken>()).Returns(false);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Handle_WhenProductIsNotUpdated_ShouldReturnConflict()
        {
            // Arrange
            var request = new UpdateProductRequest
            {
                Id = new Random().Next(1, 1000),
                Name = "Updated Product",
                CategoryId = new Random().Next(1, 1000),
            };

            _categoryRepository.CategoryExistsAsync(request.CategoryId, Arg.Any<CancellationToken>()).Returns(true);
            _productRepository.UpdateAsync(Arg.Any<ProductDto>(), Arg.Any<CancellationToken>()).Returns(false);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task Handle_WhenProductIsUpdated_ShouldReturnAccepted()
        {
            // Arrange
            var request = new UpdateProductRequest
            {
                Id = new Random().Next(1, 1000),
                Name = "Updated Product",
                CategoryId = new Random().Next(1, 1000),
            };

            _categoryRepository.CategoryExistsAsync(request.CategoryId, Arg.Any<CancellationToken>()).Returns(true);
            _productRepository.UpdateAsync(Arg.Any<ProductDto>(), Arg.Any<CancellationToken>()).Returns(true);
            _categoryRepository.GetCategoryByIdAsync(request.CategoryId, Arg.Any<CancellationToken>()).Returns(new CategoryDto
            {
                CategoryId = request.CategoryId,
                Name = "Test Category",
                LowStockThreshold = 0,
                OutOfStockThreshold = 0
            });

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.Accepted);
        }

        [Fact]
        public async Task Handle_WhenExceptionIsThrown_ShouldReturnError()
        {
            // Arrange
            var request = new UpdateProductRequest
            {
                Id = new Random().Next(1, 1000),
                Name = "Updated Product",
                CategoryId = new Random().Next(1, 1000),
            };

            _categoryRepository.CategoryExistsAsync(request.CategoryId, Arg.Any<CancellationToken>()).Returns(true);
            _productRepository.UpdateAsync(Arg.Any<ProductDto>(), Arg.Any<CancellationToken>()).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error.Message.Should().Contain("Test exception");
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.InternalServerError);
        }

    }
}
