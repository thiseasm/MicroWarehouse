﻿using MediatR;
using Microsoft.Extensions.Logging;
using MicroWarehouse.Core.Abstractions.Models.Requests.Products;
using MicroWarehouse.Core.Abstractions.Models.Responses;
using MicroWarehouse.Core.Mappings;
using MicroWarehouse.Infrastructure.Abstractions.Interfaces;

namespace MicroWarehouse.Core.Handlers.Products
{
    public class CreateProductRequestHandler(ILogger<CreateProductRequestHandler> logger, IProductRepository productRepository, ICategoryRepository categoryRepository, ICounterRepository counterRepository) 
        : IRequestHandler<CreateProductRequest, ApiResponse<int>>
    {
        public async Task<ApiResponse<int>> Handle(CreateProductRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var categoryExists = await categoryRepository.CategoryExistsAsync(request.CategoryId, cancellationToken);
                if (!categoryExists)
                {
                    var error = new Error { Message = $"Category with ID:{request.CategoryId} not found" };
                    return ApiResponse<int>.BadRequest(error);
                }

                var newProductId = await counterRepository.IncrementCounterAsync("products", cancellationToken);
                var newProduct = request.ToDto(newProductId);

                await productRepository.CreateAsync(newProduct, cancellationToken);
                return ApiResponse<int>.Created(newProductId);
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "{HandlerName} : Handle failed with Error: {Message}", nameof(CreateProductRequestHandler), ex.Message);
                var error = new Error
                {
                    Message = ex.Message
                };

                return ApiResponse<int>.InternalError(error);
            }
        }
    }
}
