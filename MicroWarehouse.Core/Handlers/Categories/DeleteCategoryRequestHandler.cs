using MediatR;
using Microsoft.Extensions.Logging;
using MicroWarehouse.Core.Abstractions.Models.Requests.Categories;
using MicroWarehouse.Core.Abstractions.Models.Responses;
using MicroWarehouse.Data.Abstractions.Interfaces;

namespace MicroWarehouse.Core.Handlers.Categories
{
    public class DeleteCategoryRequestHandler(ILogger<DeleteCategoryRequestHandler> logger, ICategoryRepository categoryRepository, IProductRepository productRepository) : IRequestHandler<DeleteCategoryRequest, ApiResponse<bool>>
    {
        public async Task<ApiResponse<bool>> Handle(DeleteCategoryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var categoryId = request.CategoryId;
                var categoryExists = await categoryRepository.ExistsAsync(categoryId, cancellationToken);
                if (!categoryExists)
                {
                    var error = new Error { Message = $"Category with ID:{categoryId} not found" };
                    return ApiResponse<bool>.NotFound(error);
                }

                var categoryExistsInProducts = await productRepository.ProductsWithCategoryExistAsync(categoryId, cancellationToken);
                if (categoryExistsInProducts)
                {
                    var error = new Error { Message = $"Cannot delete category with ID:{categoryId} because it has associated products." };
                    return ApiResponse<bool>.Conflict(error);
                }

                var result = await categoryRepository.RemoveAsync(categoryId, cancellationToken);
                return result 
                    ? ApiResponse<bool>.Ok(result) 
                    : ApiResponse<bool>.InternalError(new Error { Message = $"Failed to delete category with ID:{request.CategoryId}" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{HandlerName} : Handle failed with Error deleting category: {Message}", nameof(DeleteCategoryRequestHandler), ex.Message);
                var error = new Error
                {
                    Message = ex.Message
                };
                return ApiResponse<bool>.InternalError(error);
            }
        }
    }
}
