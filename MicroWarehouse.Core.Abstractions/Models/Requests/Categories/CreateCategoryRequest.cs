using MediatR;
using MicroWarehouse.Core.Abstractions.Models.Responses;

namespace MicroWarehouse.Core.Abstractions.Models.Requests.Categories
{
    public class CreateCategoryRequest : CategoryRequestBase, IRequest<ApiResponse<int>>
    {
    }
}
