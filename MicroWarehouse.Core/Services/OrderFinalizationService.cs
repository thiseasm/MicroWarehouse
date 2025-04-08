using Microsoft.Extensions.Logging;
using MicroWarehouse.Core.Abstractions.Enumerations;
using MicroWarehouse.Core.Abstractions.Interfaces;
using MicroWarehouse.Infrastructure.Abstractions.Interfaces;

namespace MicroWarehouse.Core.Services;

public class OrderFinalizationService(ILogger<OrderFinalizationService> logger, IOrderRepository orderRepository, IProductRepository productRepository) : IOrderFinalizationService
{
    public async Task<FinalizationStatus> ApproveOrderAsync(int orderId, CancellationToken cancellationToken)
    {
        try
        {
            var orderDto = await orderRepository.GetOrderByIdAsync(orderId, cancellationToken);
            if (orderDto is null)
            {
                logger.LogWarning("{ServiceName} : ApproveOrderAsync failed. Order with ID: {OrderId} not found", nameof(OrderFinalizationService), orderId);
                return FinalizationStatus.NotFound;
            }

            if (orderDto.StatusId != (int)OrderStatus.UnderReview)
            {
                return FinalizationStatus.AlreadyProcessed;
            }

            var result = await orderRepository.UpdateStatusAsync(orderId, (int)OrderStatus.Approved, cancellationToken);
            return result
                ? FinalizationStatus.Success
                : FinalizationStatus.UnknownError;
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "{ServiceName} : ApproveOrderAsync failed with Error: {Message}", nameof(OrderFinalizationService), ex.Message);
            return FinalizationStatus.UnknownError;
        }
    }

    public async Task<FinalizationStatus> RejectOrderAsync(int orderId, CancellationToken cancellationToken)
    {
        try
        {
            var orderDto = await orderRepository.GetOrderByIdAsync(orderId, cancellationToken);
            if (orderDto is null)
            {
                logger.LogWarning("{ServiceName} : RejectOrderAsync failed. Order with ID: {OrderId} not found", nameof(OrderFinalizationService), orderId);
                return FinalizationStatus.NotFound;
            }

            if (orderDto.StatusId != (int)OrderStatus.UnderReview)
            {
                return FinalizationStatus.AlreadyProcessed;
            }

            var freeUpStockResult = await productRepository.ReleaseStockAsync(orderDto.ProductId, orderDto.Quantity, cancellationToken);
            if (!freeUpStockResult)
            {
                logger.LogWarning("{ServiceName} : RejectOrderAsync failed. Could not release stock for Order with ID: {OrderId}", nameof(OrderFinalizationService), orderId);
                return FinalizationStatus.StockReleaseFailed;
            }

            var result = await orderRepository.UpdateStatusAsync(orderId, (int)OrderStatus.Rejected, cancellationToken);
            return result
                ? FinalizationStatus.Success
                : FinalizationStatus.UnknownError;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{ServiceName} : RejectOrderAsync failed with Error: {Message}", nameof(OrderFinalizationService), ex.Message);
            return FinalizationStatus.UnknownError;
        }
    }
}