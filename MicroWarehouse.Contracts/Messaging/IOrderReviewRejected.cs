namespace MicroWarehouse.Contracts.Messaging
{
    public interface IOrderReviewRejected
    {
        Guid CorrelationId { get; }
        int OrderId { get; }
        string Reason { get; }
    }
}
