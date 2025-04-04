
namespace MicroWarehouse.Core.Abstractions.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int Status { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}
