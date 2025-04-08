using MassTransit;
using MicroWarehouse.Contracts.Messaging;

namespace MicroWarehouse.Infrastructure.Consumers
{
    public class OrderReviewRequestedConsumer : IConsumer<IOrderReviewRequested>
    {
        public async Task Consume(ConsumeContext<IOrderReviewRequested> context)
        {
            // Simulate external system delay
            await Task.Delay(1000);

            if (context.Message.Quantity <= 5) 
            {
                await context.Publish<IOrderReviewApproved>(new
                {
                    CorrelationId = context.Message.CorrelationId,
                    OrderId = context.Message.OrderId
                });
            }
            else
            {
                await context.Publish<IOrderReviewRejected>(new
                {
                    CorrelationId = context.Message.CorrelationId,
                    OrderId = context.Message.OrderId,
                    Reason = "Quantity too high for manual approval"
                });
            }
        }
    }
}
