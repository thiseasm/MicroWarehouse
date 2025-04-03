using MediatR;
using MicroWarehouse.Core.Abstractions.Models.Responses;

namespace MicroWarehouse.Core.Abstractions.Models.Requests.Orders
{
    public class CreateOrderRequest : IRequest<ApiResponse<int>>
    {
        public List<OrderItem> Items { get; set; } = [];
    }
}
