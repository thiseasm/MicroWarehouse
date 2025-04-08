namespace MicroWarehouse.Contracts.Messaging
{
    public interface IOrderReviewRejected
    {
        Guid CorrelationId { get; }
        int OrderId { get; }
        int ProductId { get; }
        int Quantity { get; }
        string Reason { get; }
    }
}
