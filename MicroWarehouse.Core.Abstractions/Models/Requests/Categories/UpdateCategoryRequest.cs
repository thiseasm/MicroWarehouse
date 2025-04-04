using MediatR;
using MicroWarehouse.Core.Abstractions.Models.Responses;

namespace MicroWarehouse.Core.Abstractions.Models.Requests.Categories
{
    public class UpdateCategoryRequest : CategoryRequestBase, IRequest<ApiResponse<Category>>
    {
        public required int Id { get; set; }
    }
}
