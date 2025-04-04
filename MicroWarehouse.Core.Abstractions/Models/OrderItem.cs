namespace MicroWarehouse.Core.Abstractions.Models
{
    public class OrderItem
    {
        public required int ProductId { get; set; }
        public required int Quantity { get; set; }
    }
}
