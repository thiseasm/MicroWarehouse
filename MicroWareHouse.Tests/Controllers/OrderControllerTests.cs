using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MicroWarehouse.Core.Abstractions.Enumerations;
using MicroWarehouse.Core.Abstractions.Models;
using MicroWarehouse.Core.Abstractions.Models.Requests.Categories;
using MicroWarehouse.Core.Abstractions.Models.Requests.Orders;
using MicroWarehouse.Core.Abstractions.Models.Requests.Products;
using MicroWareHouse.Tests.Helpers;

namespace MicroWareHouse.Tests.Controllers
{
    public class OrderControllerTests : IClassFixture<IntegrationTestFixture>
    {
        private readonly HttpClient _client;

        public OrderControllerTests(IntegrationTestFixture fixture)
        {
            var factory = new TestApiFactory(fixture);
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetOrdersAsync_WhenOrdersExist_ShouldReturnOk()
        {
            // Act
            var response = await _client.GetAsync("/api/orders");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<IEnumerable<Order>>();
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
        }

        [Fact]
        public async Task CreateOrderAsync_WhenOrderIsValid_ShouldReturnCreated()
        {
            // Arrange
            var categoryRequest = new CreateCategoryRequest
            {
                Name = "New Category",
                LowStockThreshold = 5,
                OutOfStockThreshold = 0
            };
            var categoryResponse = await _client.PostAsJsonAsync("/api/categories", categoryRequest);
            var categoryId = await categoryResponse.Content.ReadFromJsonAsync<int>();

            var createProductRequest = new CreateProductRequest
            {
                Name = "Product for Order",
                CategoryId = categoryId,
                StockAmount = 100
            };

            var createdProductResponse = await _client.PostAsJsonAsync("/api/products", createProductRequest);
            var productId = await createdProductResponse.Content.ReadFromJsonAsync<int>();

            var request = new CreateOrderRequest
            {
                ProductId = productId,
                Quantity = 10,
                ReserveType = ReserveType.ReserveWhenAvailable
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/orders", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await response.Content.ReadFromJsonAsync<int>();
            result.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task CreateOrderAsync_WhenProductDoesNotExist_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new CreateOrderRequest
            {
                ProductId = new Random().Next(999999,int.MaxValue), 
                Quantity = 10,
                ReserveType = ReserveType.ReserveWhenAvailable
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/orders", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            result.Should().NotBeNull();
            result.Title.Should().Contain($"Product with ID: {request.ProductId} not found");
        }

        [Fact]
        public async Task CreateOrderAsync_WhenStockIsInsufficient_ShouldReturnConflict()
        {
            // Arrange
            var categoryRequest = new CreateCategoryRequest
            {
                Name = "New Category",
                LowStockThreshold = 5,
                OutOfStockThreshold = 0
            };
            var categoryResponse = await _client.PostAsJsonAsync("/api/categories", categoryRequest);
            var categoryId = await categoryResponse.Content.ReadFromJsonAsync<int>();

            var createProductRequest = new CreateProductRequest
            {
                Name = "Product with Low Stock",
                CategoryId = categoryId,
                StockAmount = 5 
            };

            var createdProductResponse = await _client.PostAsJsonAsync("/api/products", createProductRequest);
            var productId = await createdProductResponse.Content.ReadFromJsonAsync<int>();

            var request = new CreateOrderRequest
            {
                ProductId = productId,
                Quantity = 10, 
                ReserveType = ReserveType.None
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/orders", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            result.Should().NotBeNull();
            result.Title.Should().Contain("is out of stock or insufficient quantity available");
        }
    }
}

