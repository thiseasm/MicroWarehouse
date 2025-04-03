using MicroWarehouse.Core.Abstractions.Models;
using MicroWarehouse.Core.Abstractions.Models.Responses;

namespace MicroWarehouse.Core.Abstractions.Interfaces
{
    public interface IOrderService
    {
        Task<ApiResponse<int>> CreateOrderAsync(Order order, CancellationToken cancellationToken);
    }
}
