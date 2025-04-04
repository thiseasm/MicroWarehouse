namespace MicroWarehouse.Core.Abstractions.Models.Requests.Categories
{
    public abstract class CategoryRequestBase
    {
        public required string Name { get; set; }
        public required int LowStockThreshold { get; set; }
        public required int OutOfStockThreshold { get; set; }
    }
}
