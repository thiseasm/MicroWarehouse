using FluentAssertions;
using Microsoft.Extensions.Logging;
using MicroWarehouse.Core.Abstractions.Models.Requests.Categories;
using MicroWarehouse.Core.Handlers.Categories;
using MicroWarehouse.Infrastructure.Abstractions.Interfaces;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MicroWareHouse.Tests.Handlers.Categories
{
    public class CreateCategoryRequestHandlerTests
    {
        private readonly ILogger<CreateCategoryRequestHandler> _logger;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICounterRepository _counterRepository;
        private readonly CreateCategoryRequestHandler _handler;

        public CreateCategoryRequestHandlerTests()
        {
            _logger = Substitute.For<ILogger<CreateCategoryRequestHandler>>();
            _categoryRepository = Substitute.For<ICategoryRepository>();
            _counterRepository = Substitute.For<ICounterRepository>();
            _handler = new CreateCategoryRequestHandler(_logger, _categoryRepository, _counterRepository);
        }

        [Fact]
        public async Task Handle_WhenIsValid_ShouldReturnCreated()
        {
            // Arrange
            var request = new CreateCategoryRequest
            {
                Name = "Test Product",
                LowStockThreshold = 1,
                OutOfStockThreshold = 0
            };

            _counterRepository.IncrementCounterAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(new Random().Next(1,1000));

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        }

        [Fact]
        public async Task Handle_WhenExceptionIsThrown_ShouldReturnError()
        {
            // Arrange
            var request = new CreateCategoryRequest
            {
                Name = "Test Product",
                LowStockThreshold = 1,
                OutOfStockThreshold = 0
            };

            _counterRepository.IncrementCounterAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.InternalServerError);
        }
    }
}
