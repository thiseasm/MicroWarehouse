﻿using MicroWarehouse.Core.Abstractions.Models;
using MicroWarehouse.Core.Abstractions.Models.Requests.Products;
using MicroWarehouse.Data.Abstractions.DTOs;

namespace MicroWarehouse.Core.Mappings
{
    public static class ProductMappings
    {
        public static Product ToDomain(this ProductDto dto)
        {
            return new Product
            {
                Id = dto.CategoryId,
                Name = dto.Name,
                Category = null //TODO Add category mapping
            };
        }

        public static ProductDto ToDto(this CreateProductRequest request)
        {
            return new ProductDto
            {
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
