using MicroWarehouse.Core.Abstractions.Models;
using MicroWarehouse.Core.Abstractions.Models.Requests.Products;
using MicroWarehouse.Infrastructure.Abstractions.DTOs;

namespace MicroWarehouse.Core.Mappings
{
    public static class ProductMappings
    {
        public static Product ToDomain(this ProductDto dto, CategoryDto categoryDto)
        {
            return new Product
            {
                Id = dto.ProductId,
                Name = dto.Name,
                AvailableStock = dto.StockAmount,
                Category = categoryDto.ToDomain()
            };
        }

        public static ProductDto ToDto(this CreateProductRequest request, int newProductId)
        {
            return new ProductDto
            {
                ProductId = newProductId,
                Name = request.Name,
                StockAmount = request.StockAmount,
                CategoryId = request.CategoryId
            };
        }

        public static ProductDto ToDto(this UpdateProductRequest request)
        {
            return new ProductDto
            {
                ProductId = request.Id,
                Name = request.Name,
                CategoryId = request.CategoryId
            };
        }
    }
}
