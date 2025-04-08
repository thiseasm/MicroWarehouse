using MassTransit;

namespace MicroWarehouse.Core.Abstractions.States
{
    public class OrderReviewState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public int OrderId { get; set; }
        public string CurrentState { get; set; } = null!;
    }
}
