using MassTransit;
using MicroWarehouse.Contracts.Messaging;
using MicroWarehouse.Core.Abstractions.Interfaces;
using MicroWarehouse.Infrastructure.Abstractions.Sagas;

namespace MicroWarehouse.Infrastructure.Sagas
{
    public class OrderReviewStateMachine : MassTransitStateMachine<OrderReviewState>
    {
        public State AwaitingReview { get; private set; }
        public State Approved { get; private set; }
        public State Rejected { get; private set; }

        public Event<IOrderReviewRequested> OrderReviewRequested { get; private set; }
        public Event<IOrderReviewApproved> OrderReviewApproved { get; private set; }
        public Event<IOrderReviewRejected> OrderReviewRejected { get; private set; }
         
        public OrderReviewStateMachine(IOrderFinalizationService orderFinalizationService)
        {
            InstanceState(x => x.CurrentState);

            Event(() => OrderReviewRequested, x => x.CorrelateById(c => c.Message.CorrelationId));
            Event(() => OrderReviewApproved, x => x.CorrelateById(c => c.Message.CorrelationId));
            Event(() => OrderReviewRejected, x => x.CorrelateById(c => c.Message.CorrelationId));

            Initially(
                When(OrderReviewRequested)
                    .Then(context =>
                    {
                        context.Saga.OrderId = context.Message.OrderId;
                    })
                    .TransitionTo(AwaitingReview)
            );

            During(AwaitingReview,
                When(OrderReviewApproved)
                    .TransitionTo(Approved)
                    .ThenAsync(async context => await orderFinalizationService.ApproveOrderAsync(context.Message.OrderId)),

                When(OrderReviewRejected)
                    .TransitionTo(Rejected)
                    .ThenAsync(async context => await orderFinalizationService.RejectOrderAsync(context.Message.OrderId))
            );
        }
    }
}
