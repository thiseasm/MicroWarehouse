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
    public class GetAllCategoriesRequestHandlerTests
    {
        private readonly ILogger<GetAllCategoriesRequestHandler> _logger;
        private readonly ICategoryRepository _categoryRepository;
        private readonly GetAllCategoriesRequestHandler _handler;

        public GetAllCategoriesRequestHandlerTests()
        {
            _logger = Substitute.For<ILogger<GetAllCategoriesRequestHandler>>();
            _categoryRepository = Substitute.For<ICategoryRepository>();
            _handler = new GetAllCategoriesRequestHandler(_logger, _categoryRepository);
        }

        [Fact]
        public async Task Handle_WhenNoCategoriesExist_ShouldReturnEmptyList()
        {
            // Arrange
            _categoryRepository.GetAllCategoriesAsync(Arg.Any<CancellationToken>()).Returns([]);

            // Act
            var result = await _handler.Handle(new GetAllCategoriesRequest(), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            result.Data.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_WhenCategoriesExist_ShouldReturnCategories()
        {
            // Arrange
            var categories = new List<CategoryDto>
            {
                new() { CategoryId = 1, Name = "Category 1", LowStockThreshold = 5, OutOfStockThreshold = 0 },
                new() { CategoryId = 2, Name = "Category 2", LowStockThreshold = 10, OutOfStockThreshold = 2 }
            };
            _categoryRepository.GetAllCategoriesAsync(Arg.Any<CancellationToken>()).Returns(categories);

            // Act
            var result = await _handler.Handle(new GetAllCategoriesRequest(), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            result.Data.Should().HaveCount(2);
            result.Data.Should().ContainSingle(x => x.Id == 1 && x.Name == "Category 1");
            result.Data.Should().ContainSingle(x => x.Id == 2 && x.Name == "Category 2");
        }

        [Fact]
        public async Task Handle_WhenExceptionIsThrown_ShouldReturnInternalError()
        {
            // Arrange
            _categoryRepository.GetAllCategoriesAsync(Arg.Any<CancellationToken>()).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _handler.Handle(new GetAllCategoriesRequest(), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.InternalServerError);
            result.Error.Should().NotBeNull();
            result.Error.Message.Should().Contain("Database error");
        }
    }
}
