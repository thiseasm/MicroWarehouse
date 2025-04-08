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
    public class UpdateProductStockAmountRequestHandlerTests
    {
        private readonly ILogger<UpdateProductStockAmountRequestHandler> _logger;
        private readonly IProductRepository _productRepository;
        private readonly UpdateProductStockAmountRequestHandler _handler;

        public UpdateProductStockAmountRequestHandlerTests()
        {
            _logger = Substitute.For<ILogger<UpdateProductStockAmountRequestHandler>>();
            _productRepository = Substitute.For<IProductRepository>();
            _handler = new UpdateProductStockAmountRequestHandler(_logger, _productRepository);
        }

        [Fact]
        public async Task Handle_WhenProductDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var request = new UpdateProductStockAmountRequest
            {
                ProductId = new Random().Next(1,1000),
                StockAmount = 10
            };
            _productRepository.GetProductByIdAsync(request.ProductId, Arg.Any<CancellationToken>()).Returns((ProductDto)null!);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Handle_WhenProductIsNotUpdated_ShouldReturnError()
        {
            // Arrange
            var request = new UpdateProductStockAmountRequest
            {
                ProductId = new Random().Next(1, 1000),
                StockAmount = 10
            };
            _productRepository.GetProductByIdAsync(request.ProductId, Arg.Any<CancellationToken>()).Returns(new ProductDto());
            _productRepository.UpdateStockAsync(request.ProductId, request.StockAmount, Arg.Any<CancellationToken>()).Returns(false);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task Handle_WhenStockIsUpdatedSuccessfully_ShouldReturnSuccess()
        {
            // Arrange
            var request = new UpdateProductStockAmountRequest
            {
                ProductId = new Random().Next(1, 1000),
                StockAmount = 10
            };
            _productRepository.GetProductByIdAsync(request.ProductId, Arg.Any<CancellationToken>()).Returns(new ProductDto());
            _productRepository.UpdateStockAsync(request.ProductId, request.StockAmount, Arg.Any<CancellationToken>()).Returns(true);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task Handle_WhenExceptionIsThrown_ShouldReturnError()
        {
            // Arrange
            var request = new UpdateProductStockAmountRequest
            {
                ProductId = new Random().Next(1, 1000),
                StockAmount = 10
            };
            _productRepository.GetProductByIdAsync(request.ProductId, Arg.Any<CancellationToken>()).ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error.Message.Should().Contain("Test Exception");
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.InternalServerError);
        }
    }
}
