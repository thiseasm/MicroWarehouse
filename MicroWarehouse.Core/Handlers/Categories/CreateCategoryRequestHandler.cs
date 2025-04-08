using MediatR;
using Microsoft.Extensions.Logging;
using MicroWarehouse.Core.Abstractions.Models.Requests.Categories;
using MicroWarehouse.Core.Abstractions.Models.Responses;
using MicroWarehouse.Core.Mappings;
using MicroWarehouse.Infrastructure.Abstractions.Interfaces;

namespace MicroWarehouse.Core.Handlers.Categories
{
    public class CreateCategoryRequestHandler(ILogger<CreateCategoryRequestHandler> logger, ICategoryRepository categoryRepository, ICounterRepository counterRepository) 
        : IRequestHandler<CreateCategoryRequest, ApiResponse<int>>
    {
        public async Task<ApiResponse<int>> Handle(CreateCategoryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                //TODO remove magic string
                var category = request.ToDto();
                var newCategoryId = await counterRepository.IncrementCounterAsync("categories", cancellationToken);
                category.CategoryId = newCategoryId;

                await categoryRepository.CreateAsync(category, cancellationToken);
                return ApiResponse<int>.Created(newCategoryId);
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "{HandlerName} : Handle failed with Error: {Message}", nameof(CreateCategoryRequestHandler), ex.Message);
                var error = new Error
                {
                    Message = ex.Message
                };

                return ApiResponse<int>.InternalError(error);
            }
        }
    }
}
