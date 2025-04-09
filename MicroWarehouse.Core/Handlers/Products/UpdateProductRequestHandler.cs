using MediatR;
using Microsoft.Extensions.Logging;
using MicroWarehouse.Core.Abstractions.Models;
using MicroWarehouse.Core.Abstractions.Models.Requests.Products;
using MicroWarehouse.Core.Abstractions.Models.Responses;
using MicroWarehouse.Core.Mappings;
using MicroWarehouse.Infrastructure.Abstractions.Interfaces;

namespace MicroWarehouse.Core.Handlers.Products
{
    public class UpdateProductRequestHandler(ILogger<UpdateProductRequestHandler> logger, IProductRepository productRepository, ICategoryRepository categoryRepository) : IRequestHandler<UpdateProductRequest, ApiResponse<Product>>
    {
        public async Task<ApiResponse<Product>> Handle(UpdateProductRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var categoryExists = await categoryRepository.CategoryExistsAsync(request.CategoryId, cancellationToken);
                if (!categoryExists)
                {
                    var error = new Error { Message = $"Category with ID:{request.CategoryId} not found" };
                    return ApiResponse<Product>.BadRequest(error);
                }

                var productDto = request.ToDto();
                var updateResult = await productRepository.UpdateAsync(productDto, cancellationToken);

                if (!updateResult)
                {
                    var error = new Error { Message = $"Could not update product with ID:{request.Id}" };
                    return ApiResponse<Product>.Conflict(error);
                }

                var categoryDto = await categoryRepository.GetCategoryByIdAsync(request.CategoryId, cancellationToken);
                return ApiResponse<Product>.Accepted(productDto.ToDomain(categoryDto!));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{HandlerName} : Handle failed with Error: {Message}", nameof(UpdateProductRequestHandler), ex.Message);
                var error = new Error
                {
                    Message = ex.Message
                };

                return ApiResponse<Product>.InternalError(error);
            }
        }
    }
}
