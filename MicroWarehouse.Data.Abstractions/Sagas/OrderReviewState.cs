using MassTransit;

namespace MicroWarehouse.Infrastructure.Abstractions.Sagas
{
    public class OrderReviewState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public int OrderId { get; set; }
        public string CurrentState { get; set; } = null!;
    }
}
