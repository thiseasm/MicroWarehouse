using MicroWarehouse.Core.Abstractions.Enumerations;
using MicroWarehouse.Core.Abstractions.Models;
using MicroWarehouse.Core.Abstractions.Models.Requests.Orders;
using MicroWarehouse.Infrastructure.Abstractions.DTOs;

namespace MicroWarehouse.Core.Mappings
{
    public static class OrderMappings
    {
        public static OrderDto ToDto(this CreateOrderRequest request, OrderStatus status)
        {
            return new OrderDto
            {
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                OrderId = (int)status
            };
        }
        public static Order ToDomain(this OrderDto dto)
        {
            return new Order
            {
                Id = dto.OrderId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                Status = (OrderStatus)dto.StatusId
            };
        }
    }
}
