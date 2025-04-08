using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using MicroWarehouse.Core.Abstractions.Enumerations;
using MicroWarehouse.Core.Services;
using MicroWarehouse.Infrastructure.Abstractions.DTOs;
using MicroWarehouse.Infrastructure.Abstractions.Interfaces;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using static NSubstitute.Arg;

namespace MicroWareHouse.Tests.Services
{
    public class OrderFinalizationServiceTests
    {
        private readonly ILogger<OrderFinalizationService> _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly OrderFinalizationService _service;

        public OrderFinalizationServiceTests()
        {
            _logger = Substitute.For<ILogger<OrderFinalizationService>>();
            _orderRepository = Substitute.For<IOrderRepository>();
            _productRepository = Substitute.For<IProductRepository>();
            _publishEndpoint = Substitute.For<IPublishEndpoint>();
            _service = new OrderFinalizationService(_logger, _orderRepository, _productRepository, _publishEndpoint);
        }

        #region ApproveOrderAsync

        [Fact]
        public async Task ApproveOrderAsync_WhenOrderDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var orderId = new Random().Next(1,1000);
            _orderRepository.GetOrderByIdAsync(orderId, Any<CancellationToken>()).Returns((OrderDto)null!);

            // Act
            var result = await _service.ApproveOrderAsync(orderId, CancellationToken.None);

            // Assert
            result.Should().Be(FinalizationStatus.NotFound);
        }

        [Fact]
        public async Task ApproveOrderAsync_WhenOrderIsNotUnderReview_ShouldReturnAlreadyProcessed()
        {
            // Arrange
            var orderId = new Random().Next(1, 1000);
            var orderDto = new OrderDto 
            { 
                OrderId = orderId,
                StatusId = (int)OrderStatus.Approved
            };
            _orderRepository.GetOrderByIdAsync(orderId, Any<CancellationToken>()).Returns(orderDto);

            // Act
            var result = await _service.ApproveOrderAsync(orderId, CancellationToken.None);

            // Assert
            result.Should().Be(FinalizationStatus.AlreadyProcessed);
        }

        [Fact]
        public async Task ApproveOrderAsync_WhenOrderIsApproved_ShouldReturnSuccess()
        {
            // Arrange
            var orderId = new Random().Next(1, 1000);
            var orderDto = new OrderDto
            {
                OrderId = orderId,
                StatusId = (int)OrderStatus.UnderReview
            };

            _orderRepository.GetOrderByIdAsync(orderId, Any<CancellationToken>()).Returns(orderDto);
            _orderRepository.UpdateStatusAsync(orderId, (int)OrderStatus.Approved, Any<CancellationToken>()).Returns(true);

            // Act
            var result = await _service.ApproveOrderAsync(orderId, CancellationToken.None);

            // Assert
            result.Should().Be(FinalizationStatus.Success);
        }

        [Fact]
        public async Task ApproveOrderAsync_WhenUpdateFails_ShouldReturnUnknownError()
        {
            // Arrange
            var orderId = new Random().Next(1, 1000);
            var orderDto = new OrderDto
            {
                OrderId = orderId, 
                StatusId = (int)OrderStatus.UnderReview
            };

            _orderRepository.GetOrderByIdAsync(orderId, Any<CancellationToken>()).Returns(orderDto);
            _orderRepository.UpdateStatusAsync(orderId, (int)OrderStatus.Approved, Any<CancellationToken>()).Returns(false);

            // Act
            var result = await _service.ApproveOrderAsync(orderId, CancellationToken.None);

            // Assert
            result.Should().Be(FinalizationStatus.UnknownError);
        }
        
        [Fact]
        public async Task ApproveOrderAsync_WhenExceptionIsThrown_ShouldReturnUnknownError()
        {
            // Arrange
            var orderId = new Random().Next(1, 1000);
            var orderDto = new OrderDto
            {
                OrderId = orderId, 
                StatusId = (int)OrderStatus.UnderReview
            };

            _orderRepository.GetOrderByIdAsync(orderId, Any<CancellationToken>()).Returns(orderDto);
            _orderRepository.UpdateStatusAsync(orderId, (int)OrderStatus.Approved, Any<CancellationToken>()).ThrowsAsync(new Exception());

            // Act
            var result = await _service.ApproveOrderAsync(orderId, CancellationToken.None);

            // Assert
            result.Should().Be(FinalizationStatus.UnknownError);
        }

        #endregion

        #region RejectOrderAsync
        [Fact]
        public async Task RejectOrderAsync_WhenOrderDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var orderId = new Random().Next(1, 1000);
            _orderRepository.GetOrderByIdAsync(orderId, Any<CancellationToken>()).Returns((OrderDto)null!);

            // Act
            var result = await _service.RejectOrderAsync(orderId, CancellationToken.None);

            // Assert
            result.Should().Be(FinalizationStatus.NotFound);
        }

        [Fact]
        public async Task RejectOrderAsync_WhenOrderIsNotUnderReview_ShouldReturnAlreadyProcessed()
        {
            // Arrange
            var orderId = new Random().Next(1, 1000);
            var orderDto = new OrderDto { OrderId = orderId, StatusId = (int)OrderStatus.Approved };
            _orderRepository.GetOrderByIdAsync(orderId, Any<CancellationToken>()).Returns(orderDto);

            // Act
            var result = await _service.RejectOrderAsync(orderId, CancellationToken.None);

            // Assert
            result.Should().Be(FinalizationStatus.AlreadyProcessed);
        }

        [Fact]
        public async Task RejectOrderAsync_WhenStockReleaseFails_ShouldReturnStockReleaseFailed()
        {
            // Arrange
            var orderId = new Random().Next(1, 1000);
            var orderDto = new OrderDto
            {
                OrderId = orderId,
                StatusId = (int)OrderStatus.UnderReview, 
                ProductId = 1, 
                Quantity = 10
            };

            _orderRepository.GetOrderByIdAsync(orderId, Any<CancellationToken>()).Returns(orderDto);
            _productRepository.ReleaseStockAsync(orderDto.ProductId, orderDto.Quantity, Any<CancellationToken>()).Returns(false);

            // Act
            var result = await _service.RejectOrderAsync(orderId, CancellationToken.None);

            // Assert
            result.Should().Be(FinalizationStatus.StockReleaseFailed);
        }

        [Fact]
        public async Task RejectOrderAsync_WhenOrderIsRejected_ShouldReturnSuccess()
        {
            // Arrange
            var orderId = new Random().Next(1, 1000);
            var orderDto = new OrderDto
            {
                OrderId = orderId, 
                StatusId = (int)OrderStatus.UnderReview,
                ProductId = 1,
                Quantity = 10
            };

            _orderRepository.GetOrderByIdAsync(orderId, Any<CancellationToken>()).Returns(orderDto);
            _productRepository.ReleaseStockAsync(orderDto.ProductId, orderDto.Quantity, Any<CancellationToken>()).Returns(true);
            _orderRepository.UpdateStatusAsync(orderId, (int)OrderStatus.Rejected, Any<CancellationToken>()).Returns(true);

            // Act
            var result = await _service.RejectOrderAsync(orderId, CancellationToken.None);

            // Assert
            result.Should().Be(FinalizationStatus.Success);
        }

        [Fact]
        public async Task RejectOrderAsync_WhenUpdateFails_ShouldReturnUnknownError()
        {
            // Arrange
            var orderId = new Random().Next(1, 1000);
            var orderDto = new OrderDto
            {
                OrderId = orderId,
                StatusId = (int)OrderStatus.UnderReview,
                ProductId = 1,
                Quantity = 10
            };

            _orderRepository.GetOrderByIdAsync(orderId, Any<CancellationToken>()).Returns(orderDto);
            _productRepository.ReleaseStockAsync(orderDto.ProductId, orderDto.Quantity, Any<CancellationToken>()).Returns(true);
            _orderRepository.UpdateStatusAsync(orderId, (int)OrderStatus.Rejected, Any<CancellationToken>()).Returns(false);

            // Act
            var result = await _service.RejectOrderAsync(orderId, CancellationToken.None);

            // Assert
            result.Should().Be(FinalizationStatus.UnknownError);
        }
        
        [Fact]
        public async Task RejectOrderAsync_WhenExceptionIsThrown_ShouldReturnUnknownError()
        {
            // Arrange
            var orderId = new Random().Next(1, 1000);
            var orderDto = new OrderDto
            {
                OrderId = orderId,
                StatusId = (int)OrderStatus.UnderReview,
                ProductId = 1,
                Quantity = 10
            };

            _orderRepository.GetOrderByIdAsync(orderId, Any<CancellationToken>()).Returns(orderDto);
            _productRepository.ReleaseStockAsync(orderDto.ProductId, orderDto.Quantity, Any<CancellationToken>())
                .ThrowsAsync(new Exception());


            // Act
            var result = await _service.RejectOrderAsync(orderId, CancellationToken.None);

            // Assert
            result.Should().Be(FinalizationStatus.UnknownError);
        }

        #endregion
    }
}
