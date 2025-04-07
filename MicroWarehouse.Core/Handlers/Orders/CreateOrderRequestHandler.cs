using MediatR;
using MicroWarehouse.Core.Abstractions.Models;
using MicroWarehouse.Core.Abstractions.Models.Requests.Orders;
using MicroWarehouse.Core.Abstractions.Models.Responses;

namespace MicroWarehouse.Core.Handlers.Orders
{
    public class CreateOrderRequestHandler : IRequestHandler<CreateOrderRequest, ApiResponse<int>>
    {
        public async Task<ApiResponse<int>> Handle(CreateOrderRequest request, CancellationToken cancellationToken)
        {
            var order = new Order
            {
                OrderItems = request.Items
            };

            return await Task.FromResult(ApiResponse<int>.Ok(1));
        }
    }
}
