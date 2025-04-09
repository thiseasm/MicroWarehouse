using MicroWarehouse.Core.Abstractions.Enumerations;

namespace MicroWarehouse.Core.Abstractions.Models
{
    public class Order
    {
        public int Id { get; set; }
        public OrderStatus Status { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
