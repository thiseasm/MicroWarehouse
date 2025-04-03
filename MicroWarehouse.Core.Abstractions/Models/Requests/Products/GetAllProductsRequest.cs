using MediatR;
using MicroWarehouse.Core.Abstractions.Models.Responses;

namespace MicroWarehouse.Core.Abstractions.Models.Requests.Products
{
    public class GetAllProductsRequest : IRequest<ApiResponse<IEnumerable<Product>>>
    {
    }
}
