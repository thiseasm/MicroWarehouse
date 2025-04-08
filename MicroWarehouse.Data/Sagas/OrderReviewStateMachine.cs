using MassTransit;
using MicroWarehouse.Contracts.Messaging;
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

        public OrderReviewStateMachine()
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
                    .ThenAsync(context => Console.Out.WriteLineAsync($"Order {context.Saga.OrderId} approved")),

                When(OrderReviewRejected)
                    .TransitionTo(Rejected)
                    .ThenAsync(context => Console.Out.WriteLineAsync($"Order {context.Saga.OrderId} rejected"))
            );
        }
    }
}
