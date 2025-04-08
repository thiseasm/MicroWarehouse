using MediatR;
using Microsoft.Extensions.Logging;
using MicroWarehouse.Core.Abstractions.Models.Requests.Products;
using MicroWarehouse.Core.Abstractions.Models.Responses;
using MicroWarehouse.Infrastructure.Abstractions.Interfaces;

namespace MicroWarehouse.Core.Handlers.Products
{
    public class UpdateProductStockAmountRequestHandler(ILogger<UpdateProductStockAmountRequestHandler> logger, IProductRepository productRepository) 
        : IRequestHandler<UpdateProductStockAmountRequest, ApiResponse<bool>>
    {
        public async Task<ApiResponse<bool>> Handle(UpdateProductStockAmountRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var product = await productRepository.GetProductByIdAsync(request.ProductId, cancellationToken);
                if (product == null)
                {
                    var error = new Error { Message = $"Product with ID:{request.ProductId} not found" };
                    return ApiResponse<bool>.NotFound(error);
                }

                var result = await productRepository.UpdateStockAsync(request.ProductId, request.StockAmount, cancellationToken);
                if (!result)
                {
                    var error = new Error { Message = $"Failed to update stock amount for product with ID:{request.ProductId}" };
                    return ApiResponse<bool>.InternalError(error);
                }

                return ApiResponse<bool>.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{HandlerName} : Handle failed with Error: {Message}", nameof(UpdateProductStockAmountRequestHandler), ex.Message);
                var error = new Error
                {
                    Message = ex.Message
                };

                return ApiResponse<bool>.InternalError(error);
            }
        }
    }
}
