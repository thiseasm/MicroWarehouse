using MassTransit;
using Microsoft.Extensions.Logging;
using MicroWarehouse.Infrastructure.Abstractions.Interfaces;
using MicroWarehouse.Core.Abstractions.Events;
using MicroWarehouse.Core.Abstractions.Enumerations;

namespace MicroWarehouse.Infrastructure.Consumers
{
    public class StockUpdatedConsumer(ILogger<StockUpdatedConsumer> logger, IOrderRepository orderRepository, IProductRepository productRepository) : IConsumer<StockUpdated>
    {
        public async Task Consume(ConsumeContext<StockUpdated> context)
        {
            try
            {
                var productId = context.Message.ProductId;
                var pendingOrders = await orderRepository.GetPendingStockOrdersByProductAsync(productId);

                foreach (var order in pendingOrders)
                {
                    var productDto = await productRepository.GetProductByIdAsync(productId, context.CancellationToken);
                    if (productDto is null)
                    {
                        logger.LogWarning("Product with ID: {ProductId} not found. Cannot process {OrderCount}", productId, pendingOrders.Count);
                        break;
                    }

                    if (productDto.StockAmount < order.Quantity)
                    {
                        continue;
                    }

                    var reserveResult = await productRepository.ReserveStockAsync(productId, order.Quantity, context.CancellationToken);
                    if (!reserveResult)
                    {
                        logger.LogWarning("Failed to reserve stock for Order ID: {OrderId} for Product ID: {ProductId}", order.OrderId, productId);
                        continue;
                    }

                    var updateResult = await orderRepository.UpdateStatusAsync(order.OrderId, (int)OrderStatus.Approved, context.CancellationToken);

                    if (!updateResult)
                    {
                        logger.LogWarning("Failed to update Order ID: {OrderId} status to Approved", order.OrderId);
                    }

                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{ConsumerName} : Handle failed with Error: {Message}", nameof(StockUpdatedConsumer), ex.Message);
            }
        }
    }

}
