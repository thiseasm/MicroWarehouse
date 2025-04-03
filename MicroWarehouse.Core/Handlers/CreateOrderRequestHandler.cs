using MediatR;
using MicroWarehouse.Core.Abstractions.Interfaces;
using MicroWarehouse.Core.Abstractions.Models;
using MicroWarehouse.Core.Abstractions.Models.Requests.Orders;
using MicroWarehouse.Core.Abstractions.Models.Responses;

namespace MicroWarehouse.Core.Handlers
{
    public class CreateOrderRequestHandler(IOrderService orderService) : IRequestHandler<CreateOrderRequest, ApiResponse<int>>
    {
        public async Task<ApiResponse<int>> Handle(CreateOrderRequest request, CancellationToken cancellationToken)
        {
            var order = new Order
            {
                OrderItems = request.Items
            };

            return await orderService.CreateOrderAsync(order, cancellationToken);
        }
    }
}
