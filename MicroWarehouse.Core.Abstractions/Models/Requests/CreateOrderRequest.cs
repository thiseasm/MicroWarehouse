using MediatR;
using MicroWarehouse.Core.Abstractions.Models.Responses;

namespace MicroWarehouse.Core.Abstractions.Models.Requests
{
    public class CreateOrderRequest : IRequest<ApiResponse<int>>
    {
        public List<OrderItem> Items { get; set; } = [];
    }
}
