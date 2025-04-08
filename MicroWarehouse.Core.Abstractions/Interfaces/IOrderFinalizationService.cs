using MicroWarehouse.Core.Abstractions.Enumerations;

namespace MicroWarehouse.Core.Abstractions.Interfaces
{
    public interface IOrderFinalizationService
    {
        Task<FinalizationStatus> ApproveOrderAsync(int orderId, CancellationToken cancellationToken = default);
        Task<FinalizationStatus> RejectOrderAsync(int orderId, CancellationToken cancellationToken = default);
    }
}
