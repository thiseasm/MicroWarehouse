using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using MicroWarehouse.Contracts.Messaging;
using MicroWarehouse.Core.Abstractions.Enumerations;
using MicroWarehouse.Core.Abstractions.Interfaces;
using MicroWarehouse.Core.Abstractions.Models.Requests.Orders;
using MicroWarehouse.Core.Handlers.Orders;
using MicroWarehouse.Infrastructure.Abstractions.DTOs;
using MicroWarehouse.Infrastructure.Abstractions.Interfaces;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MicroWarehouse.Tests.Handlers.Orders
{
    public class CreateOrderRequestHandlerTests
    {
        private readonly ILogger<CreateOrderRequestHandler> _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IIdGeneratorService _idGeneratorService;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly CreateOrderRequestHandler _handler;

        public CreateOrderRequestHandlerTests()
        {
            _logger = Substitute.For<ILogger<CreateOrderRequestHandler>>();
            _orderRepository = Substitute.For<IOrderRepository>();
            _productRepository = Substitute.For<IProductRepository>();
            _categoryRepository = Substitute.For<ICategoryRepository>();
            _idGeneratorService = Substitute.For<IIdGeneratorService>();
            _publishEndpoint = Substitute.For<IPublishEndpoint>();
            _handler = new CreateOrderRequestHandler(
                _logger,
                _orderRepository,
                _productRepository,
                _categoryRepository,
                _idGeneratorService,
                _publishEndpoint
            );
        }

        [Fact]
        public async Task Handle_WhenProductDoesNotExist_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new CreateOrderRequest
            {
                ProductId = new Random().Next(1,1000),
                Quantity = 10
            };
            _productRepository.GetProductByIdAsync(request.ProductId, Arg.Any<CancellationToken>()).Returns((ProductDto)null!);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
            result.Error.Should().NotBeNull();
            result.Error.Message.Should().Contain($"Product with ID: {request.ProductId} not found");
        }

        [Fact]
        public async Task Handle_WhenProductIsOutOfStock_ShouldReturnConflict()
        {
            // Arrange
            var request = new CreateOrderRequest
            {
                ProductId = 1,
                Quantity = 2,
                ReserveType = ReserveType.None
            };

            var productDto = new ProductDto
            {
                ProductId = 1, 
                StockAmount = 0, 
                CategoryId = 1
            };

            var categoryDto = new CategoryDto
            {
                CategoryId = 1,
                LowStockThreshold = 20,
                OutOfStockThreshold = 5
            };

            _productRepository.GetProductByIdAsync(request.ProductId, Arg.Any<CancellationToken>()).Returns(productDto);
            _categoryRepository.GetCategoryByIdAsync(productDto.CategoryId, Arg.Any<CancellationToken>()).Returns(categoryDto);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);
            result.Error.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_WhenProductIsOutOfStockAndReserveTypeIsReserveWhenAvailable_ShouldCreatePendingOrder()
        {
            // Arrange
            var request = new CreateOrderRequest
            {
                ProductId = 1,
                Quantity = 2,
                ReserveType = ReserveType.ReserveWhenAvailable
            };

            var productDto = new ProductDto
            {
                ProductId = 1,
                StockAmount = 0,
                CategoryId = new Random().Next(1, 1000)
            };

            var categoryDto = new CategoryDto
            {
                CategoryId = 1,
                LowStockThreshold = 20,
                OutOfStockThreshold = 5
            };

            _productRepository.GetProductByIdAsync(request.ProductId, Arg.Any<CancellationToken>()).Returns(productDto);
            _categoryRepository.GetCategoryByIdAsync(productDto.CategoryId, Arg.Any<CancellationToken>()).Returns(categoryDto);
            _idGeneratorService.GetNextSequenceValueAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(1);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            result.Data.Should().Be(1);
        }

        [Fact]
        public async Task Handle_WhenStockReservationFails_ShouldReturnConflict()
        {
            // Arrange
            var request = new CreateOrderRequest
            {
                ProductId = 1,
                Quantity = 10,
                ReserveType = ReserveType.None
            };

            var productDto = new ProductDto
            {
                ProductId = 1,
                StockAmount = 20,
                CategoryId = new Random().Next(1, 1000)
            };

            var categoryDto = new CategoryDto
            {
                CategoryId = 1,
                LowStockThreshold = 20,
                OutOfStockThreshold = 5
            };

            _productRepository.GetProductByIdAsync(request.ProductId, Arg.Any<CancellationToken>()).Returns(productDto);
            _productRepository.ReserveStockAsync(request.ProductId, request.Quantity, Arg.Any<CancellationToken>()).Returns(false);
            _categoryRepository.GetCategoryByIdAsync(productDto.CategoryId, Arg.Any<CancellationToken>()).Returns(categoryDto);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);
            result.Error.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_ShouldPublishOrderReviewRequested_WhenProductIsLowOnStock()
        {
            // Arrange
            var request = new CreateOrderRequest
            {
                ProductId = 1,
                Quantity = 10,
                ReserveType = ReserveType.None
            };

            var productDto = new ProductDto
            {
                ProductId = 1,
                StockAmount = 20,
                CategoryId = 1
            };

            var categoryDto = new CategoryDto
            {
                CategoryId = 1,
                LowStockThreshold = 20, 
                OutOfStockThreshold = 5
            };

            _productRepository.GetProductByIdAsync(request.ProductId, Arg.Any<CancellationToken>()).Returns(productDto);
            _categoryRepository.GetCategoryByIdAsync(productDto.CategoryId, Arg.Any<CancellationToken>()).Returns(categoryDto);
            _productRepository.ReserveStockAsync(request.ProductId, request.Quantity, Arg.Any<CancellationToken>()).Returns(true);
            _idGeneratorService.GetNextSequenceValueAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(1);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            await _publishEndpoint.Received(1).Publish<IOrderReviewRequested>(Arg.Any<object>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WhenStockIsReservedSuccessfully_ShouldCreateOrder()
        {
            // Arrange
            var request = new CreateOrderRequest
            {
                ProductId = 1,
                Quantity = 10
            };

            var productDto = new ProductDto
            {
                ProductId = 1, 
                StockAmount = 20, 
                CategoryId = 1
            };

            var categoryDto = new CategoryDto
            {
                CategoryId = 1,
                LowStockThreshold = 8,
                OutOfStockThreshold = 5
            };

            _productRepository.GetProductByIdAsync(request.ProductId, Arg.Any<CancellationToken>()).Returns(productDto);
            _categoryRepository.GetCategoryByIdAsync(productDto.CategoryId, Arg.Any<CancellationToken>()).Returns(categoryDto);
            _productRepository.ReserveStockAsync(request.ProductId, request.Quantity, Arg.Any<CancellationToken>()).Returns(true);
            _idGeneratorService.GetNextSequenceValueAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(1);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            result.Data.Should().Be(1);
        }

        [Fact]
        public async Task Handle_ShouldReturnInternalError_WhenExceptionIsThrown()
        {
            // Arrange
            var request = new CreateOrderRequest { ProductId = 1, Quantity = 10 };
            _productRepository.GetProductByIdAsync(request.ProductId, Arg.Any<CancellationToken>()).ThrowsAsync(new Exception("Database error"));

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
