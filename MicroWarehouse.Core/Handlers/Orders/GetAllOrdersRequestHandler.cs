using MediatR;
using Microsoft.Extensions.Logging;
using MicroWarehouse.Core.Abstractions.Models;
using MicroWarehouse.Core.Abstractions.Models.Requests.Orders;
using MicroWarehouse.Core.Abstractions.Models.Responses;
using MicroWarehouse.Core.Mappings;
using MicroWarehouse.Infrastructure.Abstractions.DTOs;
using MicroWarehouse.Infrastructure.Abstractions.Interfaces;

namespace MicroWarehouse.Core.Handlers.Orders
{
    public class GetAllOrdersRequestHandler(ILogger<GetAllOrdersRequestHandler> logger, IOrderRepository orderRepository, IProductRepository productRepository) 
        : IRequestHandler<GetAllOrdersRequest, ApiResponse<IEnumerable<Order>>>
    {
        public async Task<ApiResponse<IEnumerable<Order>>> Handle(GetAllOrdersRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var allOrders = await orderRepository.GetAllOrdersAsync(cancellationToken);

                if (allOrders.Count == 0)
                {
                    return ApiResponse<IEnumerable<Order>>.Ok([]);
                }

                var orders = await GetOrdersWithProductNamesAsync(allOrders, cancellationToken);

                return ApiResponse<IEnumerable<Order>>.Ok(orders);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{HandlerName} : Handle failed with Error: {Message}", nameof(GetAllOrdersRequestHandler), ex.Message);
                var error = new Error
                {
                    Message = ex.Message
                };

                return ApiResponse<IEnumerable<Order>>.InternalError(error);
            }
        }

        private async Task<List<Order>> GetOrdersWithProductNamesAsync(List<OrderDto> allOrders, CancellationToken cancellationToken)
        {
            var products = await productRepository.GetAllProductsAsync(cancellationToken);
            var productMap = products.ToDictionary(p => p.ProductId, p => p.Name);

            var response = new List<Order>();
            foreach (var orderDto in allOrders)
            {
                var productName = productMap[orderDto.ProductId];
                response.Add(orderDto.ToDomain(productName));
            }

            return response.OrderByDescending(x => x.CreatedAt).ToList();
        }
    }
}
