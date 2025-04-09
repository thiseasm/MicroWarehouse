using MediatR;
using MicroWarehouse.Core.Abstractions.Models.Responses;

namespace MicroWarehouse.Core.Abstractions.Models.Requests.Orders
{
    public class GetAllOrdersRequest : IRequest<ApiResponse<IEnumerable<Order>>>
    {
    }
}
