namespace MicroWarehouse.Core.Abstractions.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int Status { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
