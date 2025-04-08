namespace MicroWarehouse.Contracts.Messaging
{
    public interface IOrderReviewRequested
    {
        Guid CorrelationId { get; }
        int OrderId { get; }
        int ProductId { get; }
        int Quantity { get; }
    }
}
