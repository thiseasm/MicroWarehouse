namespace MicroWarehouse.Core.Abstractions.Models
{
    public class OrderItem
    {
        public required Product Product { get; set; }
        public required int Quantity { get; set; }
    }
}
