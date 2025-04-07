using MediatR;
using MicroWarehouse.Core.Abstractions.Models.Requests.Categories;
using MicroWarehouse.Core.Abstractions.Models.Responses;
using MicroWarehouse.Data.Abstractions.DTOs;
using MicroWarehouse.Data.Abstractions.Interfaces;

namespace MicroWarehouse.Core.Handlers.Categories
{
    public class CreateCategoryRequestHandler(ICategoryRepository categoryRepository) : IRequestHandler<CreateCategoryRequest, ApiResponse<int>>
    {
        public async Task<ApiResponse<int>> Handle(CreateCategoryRequest request, CancellationToken cancellationToken)
        {
            var category = new CategoryDto()
            {
                Name = request.Name,
                LowStockThreshold = request.LowStockThreshold,
                OutOfStockThreshold = request.OutOfStockThreshold
            };

            await categoryRepository.CreateAsync(category, cancellationToken);
            return ApiResponse<int>.Created(1);
        }
    }
}
