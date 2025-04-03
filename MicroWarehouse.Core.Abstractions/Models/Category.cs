namespace MicroWarehouse.Core.Abstractions.Models
{
    public class Category
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required int LowStockThreshold { get; set; }
        public required int OutOfStockThreshold { get; set; }
    }
}
