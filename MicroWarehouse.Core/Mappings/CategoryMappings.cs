using MicroWarehouse.Core.Abstractions.Models;
using MicroWarehouse.Core.Abstractions.Models.Requests.Categories;
using MicroWarehouse.Data.Abstractions.DTOs;

namespace MicroWarehouse.Core.Mappings
{
    public static class CategoryMappings
    {
        public static Category ToDomain(this CategoryDto dto)
        {
            return new Category
            {
                Id = dto.CategoryId,
                Name = dto.Name,
                LowStockThreshold = dto.LowStockThreshold,
                OutOfStockThreshold = dto.OutOfStockThreshold
            };
        }

        public static CategoryDto ToDto(this CreateCategoryRequest request)
        {
            return new CategoryDto
            {
                Name = request.Name,
                LowStockThreshold = request.LowStockThreshold,
                OutOfStockThreshold = request.OutOfStockThreshold
            };
        }

        public static CategoryDto ToDto(this UpdateCategoryRequest request)
        {
            return new CategoryDto
            {
                CategoryId = request.Id,
                Name = request.Name,
                LowStockThreshold = request.LowStockThreshold,
                OutOfStockThreshold = request.OutOfStockThreshold
            };
        }
    }
}
