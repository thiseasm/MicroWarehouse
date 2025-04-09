using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MicroWarehouse.Core.Abstractions.Models;
using MicroWarehouse.Core.Abstractions.Models.Requests.Categories;
using MicroWareHouse.Tests.Helpers;

namespace MicroWareHouse.Tests.Controllers
{
    public class CategoryControllerTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client = factory.CreateClient();

        [Fact]
        public async Task GetCategoriesAsync_WhenCategoriesExist_ShouldReturnOk()
        {
            // Act
            var response = await _client.GetAsync("/api/categories");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<IEnumerable<Category>>();
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
        }

        [Fact]
        public async Task CreateCategoryAsync_WhenCategoryIsValid_ShouldReturnCreated()
        {
            // Arrange
            var request = new CreateCategoryRequest
            {
                Name = "New Category",
                LowStockThreshold = 5,
                OutOfStockThreshold = 0
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/categories", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await response.Content.ReadFromJsonAsync<int>();
            result.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task UpdateCategoryAsync_WhenCategoryIsUpdated_ShouldReturnAccepted()
        {
            // Arrange
            var request = new UpdateCategoryRequest
            {
                Id = 1,
                Name = "Updated Category",
                LowStockThreshold = new Random().Next(3,3000),
                OutOfStockThreshold = 2
            };

            // Act
            var response = await _client.PutAsJsonAsync("/api/categories", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Accepted);
            var result = await response.Content.ReadFromJsonAsync<Category>();
            result.Should().NotBeNull();
            result.Id.Should().Be(request.Id);
            result.Name.Should().Be(request.Name);
            result.LowStockThreshold.Should().Be(request.LowStockThreshold);
            result.OutOfStockThreshold.Should().Be(request.OutOfStockThreshold);
        }

        [Fact]
        public async Task DeleteCategoryAsync_WhenCategoryIsDeleted_ShouldReturnOk()
        {
            var createRequest = new CreateCategoryRequest
            {
                Name = "Category to be Deleted",
                LowStockThreshold = 5,
                OutOfStockThreshold = 0
            };


            var createdResponse = await _client.PostAsJsonAsync("/api/categories", createRequest);
            var categoryId = await createdResponse.Content.ReadFromJsonAsync<int>();

            var request = new DeleteCategoryRequest
            {
                CategoryId = categoryId
            };

            // Act
            var response = await _client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri("/api/categories", UriKind.Relative),
                Content = JsonContent.Create(request)
            });

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<bool>();
            result.Should().BeTrue();
        }
    }
}
