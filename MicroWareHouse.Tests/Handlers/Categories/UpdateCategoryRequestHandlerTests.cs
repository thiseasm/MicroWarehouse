using FluentAssertions;
using Microsoft.Extensions.Logging;
using MicroWarehouse.Core.Abstractions.Models.Requests.Categories;
using MicroWarehouse.Core.Handlers.Categories;
using MicroWarehouse.Infrastructure.Abstractions.DTOs;
using MicroWarehouse.Infrastructure.Abstractions.Interfaces;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MicroWarehouse.Tests.Handlers.Categories
{
    public class UpdateCategoryRequestHandlerTests
    {
        private readonly ILogger<UpdateCategoryRequestHandler> _logger;
        private readonly ICategoryRepository _categoryRepository;
        private readonly UpdateCategoryRequestHandler _handler;

        public UpdateCategoryRequestHandlerTests()
        {
            _logger = Substitute.For<ILogger<UpdateCategoryRequestHandler>>();
            _categoryRepository = Substitute.For<ICategoryRepository>();
            _handler = new UpdateCategoryRequestHandler(_logger, _categoryRepository);
        }

        [Fact]
        public async Task Handle_WhenUpdateFails_ShouldReturnConflict()
        {
            // Arrange
            var request = new UpdateCategoryRequest
            {
                Id = new Random().Next(1, 1000),
                Name = "Updated Category",
                LowStockThreshold = 5,
                OutOfStockThreshold = 0
            };

            var categoryDto = new CategoryDto
            {
                CategoryId = new Random().Next(1, 1000),
                Name = "Old Category",
                LowStockThreshold = 10,
                OutOfStockThreshold = 2
            };

            _categoryRepository.UpdateAsync(Arg.Any<CategoryDto>(), Arg.Any<CancellationToken>()).Returns(false);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task Handle_WhenCategoryIsUpdatedSuccessfully_ShouldReturnOk()
        {
            // Arrange
            var request = new UpdateCategoryRequest
            {
                Id = new Random().Next(1, 1000),
                Name = "Updated Category",
                LowStockThreshold = 5,
                OutOfStockThreshold = 0
            };

            var categoryDto = new CategoryDto
            {
                CategoryId = new Random().Next(1, 1000),
                Name = "Old Category",
                LowStockThreshold = 10,
                OutOfStockThreshold = 2
            };

            _categoryRepository.UpdateAsync(Arg.Any<CategoryDto>(), Arg.Any<CancellationToken>()).Returns(true);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        

        [Fact]
        public async Task Handle_WhenExceptionIsThrown_ShouldReturnInternalError()
        {
            // Arrange
            var request = new UpdateCategoryRequest
            {
                Id = new Random().Next(1, 1000),
                Name = "Updated Category",
                LowStockThreshold = 5,
                OutOfStockThreshold = 0
            };

            _categoryRepository.UpdateAsync(Arg.Any<CategoryDto>(), Arg.Any<CancellationToken>()).ThrowsAsync(new Exception("Database error"));

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
