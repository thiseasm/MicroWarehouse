using FluentAssertions;
using Microsoft.Extensions.Logging;
using MicroWarehouse.Core.Abstractions.Models.Requests.Products;
using MicroWarehouse.Core.Handlers.Products;
using MicroWarehouse.Infrastructure.Abstractions.Interfaces;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MicroWareHouse.Tests.Handlers.Product
{
    public class CreateProductRequestHandlerTests
    {
        private readonly ILogger<CreateProductRequestHandler> _logger;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICounterRepository _counterRepository;
        private readonly CreateProductRequestHandler _handler;

        public CreateProductRequestHandlerTests()
        {
            _logger = Substitute.For<ILogger<CreateProductRequestHandler>>();
            _productRepository = Substitute.For<IProductRepository>();
            _categoryRepository = Substitute.For<ICategoryRepository>();
            _counterRepository = Substitute.For<ICounterRepository>();
            _handler = new CreateProductRequestHandler(_logger, _productRepository, _categoryRepository, _counterRepository);
        }

        [Fact]
        public async Task Handle_WhenCategoryDoesNotExist_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new CreateProductRequest
            {
                Name = "Test Product",
                CategoryId = new Random().Next(1,1000),
                StockAmount = new Random().Next(1, 1000)
            };

            _categoryRepository.CategoryExistsAsync(request.CategoryId, Arg.Any<CancellationToken>()).Returns(false);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
            result.Error.Should().NotBeNull();
            result.Error.Message.Should().Contain($"Category with ID:{request.CategoryId} not found");
        }

        [Fact]
        public async Task Handle_WhenProductIsCreatedSuccessfully_ShouldReturnCreated()
        {
            // Arrange
            var request = new CreateProductRequest
            {
                Name = "Test Product",
                CategoryId = new Random().Next(1, 1000),
                StockAmount = new Random().Next(1, 1000)
            };

            _categoryRepository.CategoryExistsAsync(request.CategoryId, Arg.Any<CancellationToken>()).Returns(true);
            _counterRepository.IncrementCounterAsync("products", Arg.Any<CancellationToken>()).Returns(1);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            result.Data.Should().Be(1);
        }

        [Fact]
        public async Task Handle_WhenExceptionIsThrown_ShouldReturnInternalError()
        {
            // Arrange
            var request = new CreateProductRequest
            {
                Name = "Test Product",
                CategoryId = new Random().Next(1, 1000),
                StockAmount = new Random().Next(1, 1000)
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
