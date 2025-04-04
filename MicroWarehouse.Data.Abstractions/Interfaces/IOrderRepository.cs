using MicroWarehouse.Data.Abstractions.DTOs;

namespace MicroWarehouse.Data.Abstractions.Interfaces
{
    public interface IOrderRepository
    {
        public Task<List<OrderDto>> GetAllOrdersAsync(CancellationToken cancellationToken = default);

        public Task<OrderDto?> GetOrderByIdAsync(int orderId, CancellationToken cancellationToken = default);

        public Task CreateOrderAsync(OrderDto newOrder, CancellationToken cancellationToken = default);

        public Task<bool> UpdateStatusAsync(int orderId, int newStatus, CancellationToken cancellationToken = default);
    }
}
