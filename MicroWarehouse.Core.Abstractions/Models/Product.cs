namespace MicroWarehouse.Core.Abstractions.Models
{
    public class Product
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public required int AvailableStock { get; set; }
        public required Category Category { get; set; }
        
        public bool IsLowOnStock => AvailableStock <= Category.LowStockThreshold;
        public bool IsOutOfStock => AvailableStock <= Category.OutOfStockThreshold;
    }
}
