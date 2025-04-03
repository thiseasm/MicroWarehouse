using MediatR;
using MicroWarehouse.Core.Abstractions.Models.Responses;

namespace MicroWarehouse.Core.Abstractions.Models.Requests.Categories
{
    public class DeleteCategoryRequest : IRequest<ApiResponse<bool>>
    {
        public int CategoryId { get; set; }
    }
}
