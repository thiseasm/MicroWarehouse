namespace MicroWarehouse.Contracts.Messaging
{
    public interface IOrderReviewRequested
    {
        Guid CorrelationId { get; }
        int OrderId { get; }
        int Quantity { get; }
    }
}
