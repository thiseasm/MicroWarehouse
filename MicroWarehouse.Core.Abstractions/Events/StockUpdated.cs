namespace MicroWarehouse.Core.Abstractions.Events
{
    public record StockUpdated
    {
        public int ProductId { get; set; }
    }
}
