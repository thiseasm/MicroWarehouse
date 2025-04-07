using MediatR;
using Microsoft.Extensions.Logging;
using MicroWarehouse.Core.Abstractions.Models;
using MicroWarehouse.Core.Abstractions.Models.Requests.Products;
using MicroWarehouse.Core.Abstractions.Models.Responses;
using MicroWarehouse.Core.Mappings;
using MicroWarehouse.Data.Abstractions.DTOs;
using MicroWarehouse.Data.Abstractions.Interfaces;

namespace MicroWarehouse.Core.Handlers.Products
{
    public class GetAllProductsRequestHandler(ILogger<GetAllProductsRequestHandler> logger, IProductRepository productRepository, ICategoryRepository categoryRepository) : IRequestHandler<GetAllProductsRequest, ApiResponse<IEnumerable<Product>>>
    {
        public async Task<ApiResponse<IEnumerable<Product>>> Handle(GetAllProductsRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var allProducts = await productRepository.GetAllProductsAsync(cancellationToken);
                if (allProducts.Count == 0)
                {
                    return ApiResponse<IEnumerable<Product>>.Ok([]);
                }

                var response = await GetProductsWithCategoriesAsync(allProducts, cancellationToken);

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

        private async Task<List<Product>> GetProductsWithCategoriesAsync(List<ProductDto> allProducts, CancellationToken cancellationToken)
        {
            var categories = await categoryRepository.GetAllCategoriesAsync(cancellationToken);
            var categoryMap = categories.ToDictionary(c => c.CategoryId, c => c);

            var response = new List<Product>();
            foreach (var productDto in allProducts)
            {
                if (categoryMap.TryGetValue(productDto.CategoryId, out var category))
                {
                    response.Add(productDto.ToDomain(category));
                }
                else
                {
                    logger.LogWarning("Category with ID {CategoryId} not found for Product with ID: {ProductId}", productDto.CategoryId, productDto.ProductId);
                }
            }

            return response;
        }
    }
}
