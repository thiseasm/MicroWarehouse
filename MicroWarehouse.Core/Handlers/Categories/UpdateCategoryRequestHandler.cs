using MediatR;
using Microsoft.Extensions.Logging;
using MicroWarehouse.Core.Abstractions.Models;
using MicroWarehouse.Core.Abstractions.Models.Requests.Categories;
using MicroWarehouse.Core.Abstractions.Models.Responses;
using MicroWarehouse.Core.Mappings;
using MicroWarehouse.Infrastructure.Abstractions.Interfaces;


namespace MicroWarehouse.Core.Handlers.Categories
{
    public class UpdateCategoryRequestHandler(ILogger<UpdateCategoryRequestHandler> logger, ICategoryRepository categoryRepository) : IRequestHandler<UpdateCategoryRequest, ApiResponse<Category>>
    {
        public async Task<ApiResponse<Category>> Handle(UpdateCategoryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var categoryDto = request.ToDto();
                var updateResult = await categoryRepository.UpdateAsync(categoryDto, cancellationToken);

                if (!updateResult)
                {
                    var error = new Error { Message = $"Could not update category with ID:{request.Id}" };
                    return ApiResponse<Category>.Conflict(error);
                }

                return ApiResponse<Category>.Accepted(categoryDto.ToDomain());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{HandlerName} : Handle failed with Error {Message}", nameof(UpdateCategoryRequestHandler), ex.Message);
                var error = new Error
                {
                    Message = ex.Message
                };

                return ApiResponse<Category>.InternalError(error);
            }
        }
    }
}
