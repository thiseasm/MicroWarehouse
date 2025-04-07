using MediatR;
using Microsoft.Extensions.Logging;
using MicroWarehouse.Core.Abstractions.Models;
using MicroWarehouse.Core.Abstractions.Models.Requests.Products;
using MicroWarehouse.Core.Abstractions.Models.Responses;
using MicroWarehouse.Core.Mappings;
using MicroWarehouse.Data.Abstractions.Interfaces;

namespace MicroWarehouse.Core.Handlers.Products
{
    public class GetAllProductsRequestHandler(ILogger<GetAllProductsRequestHandler> logger, IProductRepository productRepository) : IRequestHandler<GetAllProductsRequest, ApiResponse<IEnumerable<Product>>>
    {
        public async Task<ApiResponse<IEnumerable<Product>>> Handle(GetAllProductsRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await productRepository.GetAllProductsAsync(cancellationToken);
                if (result.Count == 0)
                {
                    return ApiResponse<IEnumerable<Product>>.Ok([]);
                }

                var response = result.Select(x => x.ToDomain()).ToList();
                return ApiResponse<IEnumerable<Product>>.Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{HandlerName} : Handle failed with Error: {Message}", nameof(GetAllProductsRequestHandler), ex.Message);
                var error = new Error
                {
                    Message = ex.Message
                };

                return ApiResponse<IEnumerable<Product>>.InternalError(error);
            }
        }
    }
}
