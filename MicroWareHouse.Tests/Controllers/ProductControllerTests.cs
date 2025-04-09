using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MicroWarehouse.Core.Abstractions.Models;
using MicroWarehouse.Core.Abstractions.Models.Requests.Products;
using MicroWareHouse.Tests.Helpers;

namespace MicroWareHouse.Tests.Controllers
{
    public class ProductControllerTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client = factory.CreateClient();

        [Fact]
        public async Task GetProductsAsync_WhenProductsExist_ShouldReturnOk()
        {
            // Act
            var response = await _client.GetAsync("/api/products");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<IEnumerable<Product>>();
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
        }

        [Fact]
        public async Task CreateProductAsync_WhenProductIsValid_ShouldReturnCreated()
        {
            // Arrange
            var request = new CreateProductRequest
            {
                Name = "New Product",
                CategoryId = 1,
                StockAmount = 100
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/products", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await response.Content.ReadFromJsonAsync<int>();
            result.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task UpdateProductAsync_WhenProductIsUpdated_ShouldReturnAccepted()
        {
            // Arrange
            var createRequest = new CreateProductRequest
            {
                Name = "New Product",
                CategoryId = 1,
                StockAmount = 100
            };

            var createdResponse = await _client.PostAsJsonAsync("/api/products", createRequest);
            var newProductId = await createdResponse.Content.ReadFromJsonAsync<int>();

            var request = new UpdateProductRequest
            {
                Id = newProductId,
                Name = "Updated Product",
                CategoryId = 1
            };

            // Act
            var response = await _client.PutAsJsonAsync("/api/products", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Accepted);
            var result = await response.Content.ReadFromJsonAsync<Product>();
            result.Should().NotBeNull();
            result.Id.Should().Be(newProductId);
            result.Name.Should().Be(request.Name);
            result.Category.Id.Should().Be(request.CategoryId);
        }

        [Fact]
        public async Task UpdateProductStockAsync_WhenStockIsUpdated_ShouldReturnAccepted()
        {
            var createRequest = new CreateProductRequest
            {
                Name = "New Product",
                CategoryId = 1,
                StockAmount = 100
            };

            var createdResponse = await _client.PostAsJsonAsync("/api/products", createRequest);
            var newProductId = await createdResponse.Content.ReadFromJsonAsync<int>();

            // Arrange
            var request = new UpdateProductStockAmountRequest
            {
                ProductId = newProductId,
                StockAmount = 50
            };

            // Act
            var response = await _client.PatchAsync("/api/products/stock", JsonContent.Create(request));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Accepted);
            var result = await response.Content.ReadFromJsonAsync<bool>();
            result.Should().BeTrue();
        }
    }
}
