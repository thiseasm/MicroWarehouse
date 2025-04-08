using MediatR;
using Microsoft.Extensions.Logging;
using MicroWarehouse.Core.Abstractions.Models;
using MicroWarehouse.Core.Abstractions.Models.Requests.Categories;
using MicroWarehouse.Core.Abstractions.Models.Responses;
using MicroWarehouse.Core.Mappings;
using MicroWarehouse.Infrastructure.Abstractions.Interfaces;

namespace MicroWarehouse.Core.Handlers.Categories
{
    public class GetAllCategoriesRequestHandler(ILogger<GetAllCategoriesRequest> logger, ICategoryRepository categoryRepository) : IRequestHandler<GetAllCategoriesRequest, ApiResponse<IEnumerable<Category>>>
    {
        public async Task<ApiResponse<IEnumerable<Category>>> Handle(GetAllCategoriesRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await categoryRepository.GetAllCategoriesAsync(cancellationToken);
                if (result.Count == 0)
                {
                    return ApiResponse<IEnumerable<Category>>.Ok([]);
                }

                var response = result.Select(x => x.ToDomain()).ToList();
                return ApiResponse<IEnumerable<Category>>.Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{HandlerName} : Handle failed with Error: {Message}", nameof(GetAllCategoriesRequestHandler), ex.Message);
                var error = new Error
                {
                    Message = ex.Message
                };

                return ApiResponse<IEnumerable<Category>>.InternalError(error);
            }
        }
    }
}
