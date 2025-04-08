using FluentAssertions;
using Microsoft.Extensions.Logging;
using MicroWarehouse.Core.Abstractions.Models.Requests.Products;
using MicroWarehouse.Core.Handlers.Products;
using MicroWarehouse.Infrastructure.Abstractions.DTOs;
using MicroWarehouse.Infrastructure.Abstractions.Interfaces;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MicroWareHouse.Tests.Handlers.Product
{
    public class GetAllProductsRequestHandlerTests
    {
        private readonly ILogger<GetAllProductsRequestHandler> _logger;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly GetAllProductsRequestHandler _handler;

        public GetAllProductsRequestHandlerTests()
        {
            _logger = Substitute.For<ILogger<GetAllProductsRequestHandler>>();
            _productRepository = Substitute.For<IProductRepository>();
            _categoryRepository = Substitute.For<ICategoryRepository>();
            _handler = new GetAllProductsRequestHandler(_logger, _productRepository, _categoryRepository);
        }

        [Fact]
        public async Task Handle_WhenNoProductsExist_ShouldReturnEmptyList()
        {
            // Arrange
            _productRepository.GetAllProductsAsync(Arg.Any<CancellationToken>()).Returns([]);

            // Act
            var result = await _handler.Handle(new GetAllProductsRequest(), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_WhenProductsExist_ShouldReturnProducts()
        {
            // Arrange
            var products = new List<ProductDto>
            {
                new() { ProductId = 1, Name = "Product 1", StockAmount = 10, CategoryId = 1 },
                new() { ProductId = 2, Name = "Product 2", StockAmount = 20, CategoryId = 2 }
            };

            _productRepository.GetAllProductsAsync(Arg.Any<CancellationToken>()).Returns(products);
            _categoryRepository.GetAllCategoriesAsync(Arg.Any<CancellationToken>()).Returns([
                new CategoryDto { CategoryId = 1, Name = "Category 1" },
                new CategoryDto { CategoryId = 2, Name = "Category 2" }
            ]);

            // Act
            var result = await _handler.Handle(new GetAllProductsRequest(), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().HaveCount(2);
        }

        [Fact]
        public async Task Handle_WhenExceptionIsThrown_ShouldReturnError()
        {
            // Arrange

            _productRepository.GetAllProductsAsync(Arg.Any<CancellationToken>()).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _handler.Handle(new GetAllProductsRequest(), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error.Message.Should().Contain("Test exception");
        }
    }
}
