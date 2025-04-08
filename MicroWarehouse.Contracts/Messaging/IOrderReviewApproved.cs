namespace MicroWarehouse.Contracts.Messaging
{
    public interface IOrderReviewApproved
    {
        Guid CorrelationId { get; }
        int OrderId { get; }
    }
}
